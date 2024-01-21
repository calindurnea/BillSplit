import {createCookieSessionStorage, redirect} from '@remix-run/node'
import {ENV} from '~/env'

type SessionData = {
  userId: string
  email: string
  token: string
  expiresOn: Date
}

type SessionFlashData = {
  error: string
}

export const {getSession, commitSession, destroySession} =
  createCookieSessionStorage<SessionData, SessionFlashData>({
    cookie: {
      name: '__session',
      secure: true,
      httpOnly: true,
      sameSite: 'lax',
      path: '/',
      secrets: [ENV.SESSION_SECRET],
    },
  })

class AuthenticationError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'AuthenticationError'
  }
}

export async function authFetch(
  token: string,
  request: Request,
  url: string,
  init?: RequestInit,
) {
  const session = await getSession(request.headers.get('Cookie'))
  const response = await fetch(url, {
    ...init,
    headers: {
      ...init?.headers,
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  // Token might still be around, but not valid anymore
  if (response.status === 401 || response.status === 403) {
    throw redirect('/login', {
      headers: {'Set-Cookie': await destroySession(session)},
    })
  }

  return response
}

export async function authenticate(request: Request) {
  const session = await getSession(request.headers.get('Cookie'))

  try {
    const accessToken = session.get('token')
    const expiresOn = session.get('expiresOn')

    if (!accessToken) throw redirect('/login')
    if (expiresOn && new Date(expiresOn) < new Date()) {
      throw new AuthenticationError('Expired')
    }

    return accessToken
  } catch (error) {
    if (error instanceof AuthenticationError) {
      console.error('Session expired, destroying session')

      throw redirect('/login', {
        headers: {'Set-Cookie': await destroySession(session)},
      }) // TODO: (gonza) - Remove this when refresh token is implemented

      // // refresh the token somehow, this depends on the API you are using
      // const {accessToken, refreshToken, expirationDate} = await refreshToken(
      //   session.get('refreshToken'),
      // )
      //
      // // update the session with the new values
      // session.set('accessToken', accessToken)
      // session.set('refreshToken', refreshToken)
      // session.set('expirationDate', expirationDate)
      //
      // // commit the session and append the Set-Cookie header
      // headers.append('Set-Cookie', await commitSession(session))
      //
      // // redirect to the same URL if the request was a GET (loader)
      // if (request.method === 'GET') throw redirect(request.url, {headers})
      //
      // // return the access token so you can use it in your action
      // return accessToken
    }

    // throw again any unexpected error that could've happened
    throw error
  }
}
