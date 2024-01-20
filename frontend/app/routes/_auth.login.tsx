import type {ActionFunctionArgs} from '@remix-run/node'
import {Form, Link, json, redirect} from '@remix-run/react'
import {z} from 'zod'
import {Button} from '~/components/ui/button'
import {Input} from '~/components/ui/input'
import {Label} from '~/components/ui/label'
import {commitSession, getSession} from '~/utils/session.server'

const loginSchema = z.object({
  email: z.string().email(),
  password: z.string().trim().min(0),
})

export async function action({request}: ActionFunctionArgs) {
  const session = await getSession(request.headers.get('Cookie'))
  const formData = Object.fromEntries(await request.formData())

  const parsedForm = loginSchema.safeParse(formData)
  if (!parsedForm.success) {
    const fieldErrors = parsedForm.error.flatten().fieldErrors
    return json({type: 'registerFormError' as const, error: {fieldErrors}})
  }
  const {data: body} = parsedForm
  const response = await fetch(
    'http://localhost:5003/api/Authorization/login',
    {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
      },
    },
  )
  const data = (await response.json()) as {token: string; expiresOn: Date}

  session.set('token', data.token)
  session.set('expiresOn', data.expiresOn)
  session.unset('userId')
  session.unset('email')

  return redirect('/', {
    headers: {
      'Set-Cookie': await commitSession(session),
    },
  })
}

export default function Login() {
  return (
    <div>
      <div className="mb-6 text-center">
        <h1 className="text-3xl font-bold">Login</h1>
        <p className="text-muted-foreground">
          Enter your email and password to login to your account
        </p>
      </div>

      <Form method="post">
        <div className="mb-4 flex flex-col gap-2">
          <Label htmlFor="email">Email</Label>
          <Input id="email" name="email" type="email" />
        </div>
        <div className="mb-4 flex flex-col gap-2">
          <Label htmlFor="email">Password</Label>
          <Input id="password" name="password" type="password" />
        </div>

        <Button className="w-full" variant="default" type="submit">
          Login
        </Button>
      </Form>

      <div className="mt-6 text-center">
        <p className="text-muted-foreground">Don&apos;t have an account?</p>
        <Link className="text-dark-foreground underline" to="/register">
          Register
        </Link>
      </div>
    </div>
  )
}
