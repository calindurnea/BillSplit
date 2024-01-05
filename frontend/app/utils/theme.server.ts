import * as cookie from 'cookie'

const themes = ['dark', 'light', 'system'] as const
export type Theme = (typeof themes)[number]

export function isValidTheme(value: unknown): value is Theme {
  return typeof value === 'string' && themes.includes(value as Theme)
}

const cookieName = 'theme'

/** @returns the cookie value set by server (`undefined` if theme is system) */
export function getTheme(
  request: Request,
): Exclude<Theme, 'system'> | undefined {
  const cookieHeader = request.headers.get('Cookie')
  const parsed = cookieHeader && cookie.parse(cookieHeader)[cookieName]
  if (parsed === 'light' || parsed === 'dark') return parsed
}

/** Parses the cookie value */
export function setTheme(theme: Theme) {
  if (theme === 'system') {
    return cookie.serialize(cookieName, '', {path: '/', maxAge: -1})
  } else {
    return cookie.serialize(cookieName, theme, {path: '/', maxAge: 31536000})
  }
}
