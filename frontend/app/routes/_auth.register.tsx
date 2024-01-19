import type {ActionFunctionArgs} from '@remix-run/node'
import {
  Form,
  Link,
  isRouteErrorResponse,
  json,
  redirect,
  useActionData,
  useNavigation,
  useRouteError,
} from '@remix-run/react'
import {AlertCircle, Loader2} from 'lucide-react'
import {z} from 'zod'
import {Alert, AlertDescription, AlertTitle} from '~/components/ui/alert'
import {Button} from '~/components/ui/button'
import {Input} from '~/components/ui/input'
import {Label} from '~/components/ui/label'
import {knownErrorSchema} from '~/utils/types'

const formSchema = z.object({
  name: z.string().trim().min(1),
  phoneNumber: z.string().trim().min(1),
  email: z.string().email(),
})

const KNWON_ERROR_STATUS = [400, 401, 403, 409]

export async function action({request}: ActionFunctionArgs) {
  const formData = Object.fromEntries(await request.formData())

  const parsedForm = formSchema.safeParse(formData)
  if (!parsedForm.success) {
    const fieldErrors = parsedForm.error.flatten().fieldErrors
    return json({type: 'formError', error: {fieldErrors}} as const)
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
    return redirect('/login')
  }

  if (KNWON_ERROR_STATUS.includes(response.status)) {
    const responseError = knownErrorSchema.parse(await response.json())
    return json({type: 'responseError', error: {responseError}} as const)
  }

  throw new Response(null, {status: response.status})
}

export default function Register() {
  const actionData = useActionData<typeof action>()
  const navigation = useNavigation()
  const isFormInvalid = actionData?.type === 'formError'
  const isResponseError = actionData?.type === 'responseError'

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
              {actionData?.error.responseError.detail}
            </AlertDescription>
          </Alert>
        </div>
      ) : null}

      <Form method="post">
        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-2">
            <Label htmlFor="name">Name</Label>
            <Input id="name" name="name" placeholder="John Doe" />
            {isFormInvalid && actionData?.error.fieldErrors?.name ? (
              <p className="text-sm text-red-500">Field is required</p>
            ) : null}
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="phone">Phone</Label>
            <Input id="phone" name="phoneNumber" placeholder="+1 234 567 890" />
            {isFormInvalid && actionData?.error.fieldErrors?.phoneNumber ? (
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
          {isFormInvalid && actionData?.error.fieldErrors?.email ? (
            <p className="text-sm text-red-500">Invalid email address</p>
          ) : null}
        </div>

        <Button className="mt-4 w-full" type="submit">
          {navigation.state === 'submitting' ? (
            <Loader2 className="mr-2 animate-spin" />
          ) : null}
          Register
        </Button>
      </Form>

      <div className="mt-6 text-center">
        <p className="text-muted-foreground">Already have an account?</p>
        <Link className="text-dark-foreground underline" to="/login">
          Login
        </Link>
      </div>
    </div>
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
