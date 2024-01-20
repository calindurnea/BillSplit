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

async function authenticate(request: Request, headers = new Headers()) {
  const session = await getSession(request.headers.get('Cookie'))
  try {
    // get the auth data from the session
    const accessToken = session.get('token')
    const expiresOn = session.get('expiresOn')

    // if not found, redirect to login, this means the user is not even logged-in
    if (!accessToken) {
      headers.delete('Authorization')
      throw redirect('/login')
    }

    // if expired throw an error (we can extends Error to create this)
    if (expiresOn && new Date(expiresOn) < new Date()) {
      throw new AuthorizationError('Expired')
    }

    headers.set('Authorization', `Bearer ${accessToken}`)

    // if not expired, return the access token
    // return accessToken
  } catch (error) {
    // // here, check if the error is an AuthorizationError (the one we throw above)
    // if (error instanceof AuthorizationError) {
    //   // refresh the token somehow, this depends on the API you are using
    //   const {accessToken, refreshToken, expirationDate} = await refreshToken(
    //     session.get('refreshToken'),
    //   )
    //
    //   // update the session with the new values
    //   session.set('accessToken', accessToken)
    //   session.set('refreshToken', refreshToken)
    //   session.set('expirationDate', expirationDate)
    //
    //   // commit the session and append the Set-Cookie header
    //   headers.append('Set-Cookie', await commitSession(session))
    //
    //   // redirect to the same URL if the request was a GET (loader)
    //   if (request.method === 'GET') throw redirect(request.url, {headers})
    //
    //   // return the access token so you can use it in your action
    //   return accessToken
    // }
    //
    // // throw again any unexpected error that could've happened
    // throw error
  }
}
