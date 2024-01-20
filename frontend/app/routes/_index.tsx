import type {LoaderFunctionArgs} from '@remix-run/node'
import {authenticate} from '~/utils/session.server'

export async function loader({request}: LoaderFunctionArgs) {
  await authenticate(request)
  return null
}

export default function Index() {
  return (
    <div>
      <h1>Hello world!</h1>
    </div>
  )
}
