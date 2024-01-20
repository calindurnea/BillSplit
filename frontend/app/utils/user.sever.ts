import cookie from 'cookie'

const cookieName = 'user'

export function getRegisterUser(request: Request) {
  const cookieHeader = request.headers.get('cookie')
  const userId = cookieHeader
    ? cookie.parse(cookieHeader)[cookieName]
    : undefined
  return userId
}

export function setRegisterUser(
  user: {userId: number; email: string} | undefined,
) {
  if (!user) {
    return cookie.serialize(cookieName, '', {maxAge: -1, path: '/register'})
  }

  return cookie.serialize(cookieName, JSON.stringify(user), {
    path: '/register',
    httpOnly: true,
    sameSite: 'lax',
  })
}
