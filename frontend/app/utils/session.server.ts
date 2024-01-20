import {createCookieSessionStorage} from '@remix-run/node'

type SessionData = {
  userId: string
  email: string
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
