export async function authFetch(
  token: string,
  url: string,
  init?: RequestInit,
) {
  const response = await fetch(url, {
    ...init,
    headers: {
      ...init?.headers,
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  })

  // Token might still be around, but not valid anymore
  if (response.status === 401) {
    throw new Error('Unauthorized')
  }

  return response
}
