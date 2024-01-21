import type {LoaderFunctionArgs} from '@remix-run/node'
import {authFetch, authenticate} from '~/utils/session.server'

export async function loader({request}: LoaderFunctionArgs) {
  const token = await authenticate(request)
  try {
    const response = await authFetch(
      token,
      request,
      'http://localhost:5003/api/Users',
    )
    const data = await response.json()
    console.log('logging data: ', data)
  } catch (error) {
    console.error(error)
    throw error
  }
  return null
}

export default function Index() {
  return (
    <div>
      <h1>Hello world!</h1>
    </div>
  )
}
