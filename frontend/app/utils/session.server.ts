import {createCookieSessionStorage, redirect} from '@remix-run/node'

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
    },
  })

class AuthorizationError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'ValidationError'
  }
}

export async function authenticate(request: Request) {
  const session = await getSession(request.headers.get('Cookie'))

  try {
    const accessToken = session.get('token')
    const expiresOn = session.get('expiresOn')

    if (!accessToken) throw redirect('/login')
    if (expiresOn && new Date(expiresOn) < new Date()) {
      throw new AuthorizationError('Expired')
    }

    return accessToken
  } catch (error) {
    if (error instanceof AuthorizationError) {
      await destroySession(session)
      throw redirect('/login') // TODO: (gonza) - Remove this when refresh token is implemented
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
