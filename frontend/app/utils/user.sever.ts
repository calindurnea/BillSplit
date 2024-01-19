import cookie from 'cookie'

const cookieName = 'userId'

export function getUserId(request: Request) {
  const cookieHeader = request.headers.get('cookie')
  const userId = cookieHeader
    ? cookie.parse(cookieHeader)[cookieName]
    : undefined
  return userId
}

export function setUserId(userId?: number) {
  if (!userId) {
    return cookie.serialize(cookieName, '', {maxAge: -1, path: '/register'})
  }

  return cookie.serialize(cookieName, String(userId), {
    path: '/register',
    httpOnly: true,
    sameSite: 'lax',
  })
}
