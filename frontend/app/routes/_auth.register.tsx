import type {ActionFunctionArgs, LoaderFunctionArgs} from '@remix-run/node'
import {
  Form,
  Link,
  isRouteErrorResponse,
  json,
  redirect,
  useActionData,
  useLoaderData,
  useNavigation,
  useRouteError,
} from '@remix-run/react'
import {AlertCircle, Loader2} from 'lucide-react'
import {z} from 'zod'
import {Alert, AlertDescription, AlertTitle} from '~/components/ui/alert'
import {Button} from '~/components/ui/button'
import {Input} from '~/components/ui/input'
import {Label} from '~/components/ui/label'
import {
  authenticate,
  commitSession,
  destroySession,
  getSession,
} from '~/utils/session.server'
import {entitySchema, knownErrorSchema} from '~/utils/types'

const registerSchema = z.object({
  name: z.string().trim().min(1),
  phoneNumber: z.string().trim().min(1),
  email: z.string().email(),
})

const passwordSchema = z
  .object({
    password: z
      .string()
      .min(6, {message: 'Password must contain at least 6 characters'}),
    passwordCheck: z
      .string()
      .min(6, {message: 'Password must contain at least 6 characters'}),
  })
  .refine(
    schema => schema.password === schema.passwordCheck,
    'Passwords must match',
  )

const KNWON_REGISTER_ERROR_STATUS = [400, 401, 403, 409]
const KNWON_PASSWORD_ERROR_STATUS = [400, 401, 403, 404]

export async function action({request}: ActionFunctionArgs) {
  const session = await getSession(request.headers.get('Cookie'))
  const formData = Object.fromEntries(await request.formData())
  const {intent} = z
    .object({
      intent: z.union([
        z.literal('register'),
        z.literal('password'),
        z.literal('destroy'),
      ]),
    })
    .parse(formData)

  if (intent === 'destroy') {
    return json(
      {type: 'destroy' as const},
      {headers: {'Set-Cookie': await destroySession(session)}},
    )
  }

  if (intent === 'password') {
    const parsedForm = passwordSchema.safeParse(formData)
    if (!parsedForm.success) {
      const fieldErrors = parsedForm.error.flatten().fieldErrors
      const formErrors = parsedForm.error.flatten().formErrors
      return json({
        type: 'passwordFormError' as const,
        error: {fieldErrors, formErrors},
      })
    }
    const {data: body} = parsedForm

    const response = await fetch(
      'http://localhost:5003/api/Authorization/password',
      {
        method: 'POST',
        body: JSON.stringify({...body, userId: session.get('userId')}),
        headers: {
          'Content-Type': 'application/json',
        },
      },
    )

    // TODO: If successful, we set the password and automatically login the user
    if (response.status === 204) {
      const response = await fetch(
        'http://localhost:5003/api/Authorization/login',
        {
          method: 'POST',
          body: JSON.stringify({
            email: session.get('email'),
            password: body.password,
          }),
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
          Authorization: `Bearer ${data.token}`,
          'Set-Cookie': await commitSession(session),
        },
      })
    }

    if (KNWON_PASSWORD_ERROR_STATUS.includes(response.status)) {
      const responseError = knownErrorSchema.parse(await response.json())
      return json({type: 'responseError' as const, error: {responseError}})
    }
  }

  if (intent === 'register') {
    const parsedForm = registerSchema.safeParse(formData)
    if (!parsedForm.success) {
      const fieldErrors = parsedForm.error.flatten().fieldErrors
      return json({type: 'registerFormError' as const, error: {fieldErrors}})
    }
    const {data: body} = parsedForm

    const response = await fetch('http://localhost:5003/api/Users', {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (response.status === 201) {
      const {id} = entitySchema.parse(await response.json())
      session.set('userId', String(id))
      session.set('email', body.email)
      const responseInit = {
        headers: {'Set-Cookie': await commitSession(session)},
      }
      return json({type: 'registerSuccess' as const, ...body}, responseInit)
    }

    if (KNWON_REGISTER_ERROR_STATUS.includes(response.status)) {
      const responseError = knownErrorSchema.parse(await response.json())
      return json({type: 'responseError' as const, error: {responseError}})
    }
  }
}

export async function loader({request}: LoaderFunctionArgs) {
  const session = await getSession(request.headers.get('Cookie'))
  await authenticate(request)
  return json({userId: session.get('userId'), email: session.get('email')})
}

export default function RegisterPage() {
  const {userId} = useLoaderData<typeof loader>()
  const actionData = useActionData<typeof action>()
  const isResponseError = actionData?.type === 'responseError'
  const isRegisterSuccess = Boolean(userId)

  return (
    <div>
      <div className="mb-6 text-center">
        <h1 className="text-3xl font-bold">Register</h1>
        <p className="text-muted-foreground">
          Enter your information to create an account
        </p>
      </div>

      {isResponseError ? (
        <div className="mb-6">
          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>Something went wrong</AlertTitle>
            <AlertDescription>
              {actionData?.error?.responseError.detail}
            </AlertDescription>
          </Alert>
        </div>
      ) : null}

      {isRegisterSuccess ? <PasswordForm /> : <RegisterForm />}

      <div className="mt-6 text-center">
        <p className="text-muted-foreground">Already have an account?</p>
        <Link className="text-dark-foreground underline" to="/login">
          Login
        </Link>
      </div>
    </div>
  )
}

function RegisterForm() {
  const actionData = useActionData<typeof action>()
  const navigation = useNavigation()
  const isRegisterFormInvalid = actionData?.type === 'registerFormError'

  return (
    <Form method="post">
      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="name">Name</Label>
          <Input id="name" name="name" placeholder="John Doe" />
          {isRegisterFormInvalid && actionData?.error.fieldErrors?.name ? (
            <p className="text-sm text-red-500">Field is required</p>
          ) : null}
        </div>

        <div className="flex flex-col gap-2">
          <Label htmlFor="phone">Phone</Label>
          <Input id="phone" name="phoneNumber" placeholder="+1 234 567 890" />
          {isRegisterFormInvalid &&
          actionData?.error.fieldErrors?.phoneNumber ? (
            <p className="text-sm text-red-500">Field is required</p>
          ) : null}
        </div>
      </div>

      <div className="mt-4 flex flex-col gap-2">
        <Label htmlFor="email">Email</Label>
        <Input
          id="email"
          name="email"
          type="email"
          placeholder="m@example.com"
        />
        {isRegisterFormInvalid && actionData?.error.fieldErrors?.email ? (
          <p className="text-sm text-red-500">Invalid email address</p>
        ) : null}
      </div>

      <Button
        className="mt-4 w-full"
        type="submit"
        name="intent"
        value="register"
      >
        {navigation.state === 'submitting' ? (
          <Loader2 className="mr-2 animate-spin" />
        ) : null}
        Register
      </Button>
    </Form>
  )
}

function PasswordForm() {
  const {userId, email} = useLoaderData<typeof loader>()
  const actionData = useActionData<typeof action>()
  const isPasswordFormInvalid = actionData?.type === 'passwordFormError'

  return (
    <Form method="post">
      <input type="hidden" value={userId} name="userId" />
      <input type="hidden" value={email} name="email" />
      <div className="flex flex-col gap-2">
        <Label htmlFor="password">Password</Label>
        <Input id="password" name="password" type="password" />
        {isPasswordFormInvalid && actionData?.error.fieldErrors?.password ? (
          <p className="text-sm text-red-500">
            {actionData.error.fieldErrors.password[0]}
          </p>
        ) : null}
      </div>
      <div className="mt-4 flex flex-col gap-2">
        <Label htmlFor="passwordCheck">Repeat password</Label>
        <Input id="passwordCheck" name="passwordCheck" type="password" />
        <div>
          {isPasswordFormInvalid && actionData?.error.fieldErrors?.password ? (
            <p className="text-sm text-red-500">
              {actionData.error.fieldErrors.password[0]}
            </p>
          ) : null}
          {isPasswordFormInvalid && actionData.error.formErrors ? (
            <p className="text-sm text-red-500">
              {actionData.error.formErrors[0]}
            </p>
          ) : null}
        </div>
      </div>

      <div className="mt-4 flex items-center gap-2">
        <Button
          className="flex-1"
          variant="outline"
          name="intent"
          value="destroy"
          type="submit"
        >
          Register
        </Button>
        <Button className="flex-1" type="submit" name="intent" value="password">
          Set password
        </Button>
      </div>
    </Form>
  )
}

export function ErrorBoundary() {
  const error = useRouteError()

  if (typeof document !== 'undefined') {
    console.error(error)
  }

  if (isRouteErrorResponse(error)) {
    return (
      <div className="flex items-center justify-center">
        <div>
          <h1 className="text-5xl font-bold">Oops!</h1>
          {error.data?.message ? (
            <p className="my-4 text-xl text-muted-foreground">
              {error.data.message}
            </p>
          ) : (
            'An error occurred. Please try again later.'
          )}
          <Link to="/register">
            <Button variant="secondary">Try again</Button>
          </Link>
        </div>
      </div>
    )
  }

  if (error instanceof Error) {
    return (
      <div className="flex items-center justify-center">
        <div>
          <h1 className="text-5xl font-bold">Oops!</h1>
          <p className="my-4 text-xl text-muted-foreground">
            An error occurred. Please try again later.
          </p>
          <Link to="/register">
            <Button variant="secondary">Try again</Button>
          </Link>
        </div>
      </div>
    )
  }

  throw new Error('Unhandled error in error boundary')
}
