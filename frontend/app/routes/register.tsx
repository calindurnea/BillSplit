import type {ActionFunctionArgs} from '@remix-run/node'
import {
  Form,
  isRouteErrorResponse,
  redirect,
  useActionData,
  useNavigation,
  useRouteError,
} from '@remix-run/react'
import {Loader2} from 'lucide-react'
import {z} from 'zod'
import {Button} from '~/components/ui/button'
import {Input} from '~/components/ui/input'
import {Label} from '~/components/ui/label'

const formSchema = z.object({
  name: z.string().trim().min(1),
  phoneNumber: z.string().trim().min(1),
  email: z.string().email(),
})

export async function action({request}: ActionFunctionArgs) {
  const formData = Object.fromEntries(await request.formData())

  const parsedForm = formSchema.safeParse(formData)
  if (!parsedForm.success) {
    const errors = parsedForm.error.flatten().fieldErrors
    return {errors}
  }
  const {data: body} = parsedForm

  const response = await fetch('http://localhost:5003/api/Users', {
    method: 'POST',
    body: JSON.stringify(body),
    headers: {
      'Content-Type': 'application/json',
    },
  })

  switch (response.status) {
    case 201: {
      return redirect('/login')
    }

    case 400: {
      throw new Response(null, {
        status: 400,
        statusText: `Username/email ${body.email} already exists`,
      })
    }

    case 409: {
      throw new Response(null, {
        status: 409,
        statusText: `Username/email ${body.email} already exists`,
      })
    }

    default: {
      throw new Response(null, {
        status: response.status,
        statusText: response.statusText,
      })
    }
  }
}

export default function Register() {
  const actionData = useActionData<typeof action>()
  const navigation = useNavigation()

  return (
    <div className="mx-auto mt-6 max-w-sm">
      <div className="mb-6 text-center">
        <h1 className="text-3xl font-bold">Register</h1>
        <p className="text-muted-foreground">
          Enter your information to create an account
        </p>
      </div>

      <Form method="post">
        <div className="grid grid-cols-2 gap-4">
          <div className="flex flex-col gap-2">
            <Label htmlFor="name">Name</Label>
            <Input id="name" name="name" placeholder="John Doe" />
            {actionData?.errors?.name ? (
              <p className="text-sm text-red-500">Field is required</p>
            ) : null}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="phone">Phone</Label>
            <Input id="phone" name="phoneNumber" placeholder="+1 234 567 890" />
            {actionData?.errors?.phoneNumber ? (
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
          {actionData?.errors?.email ? (
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
    </div>
  )
}

export function ErrorBoundary() {
  const error = useRouteError()

  if (isRouteErrorResponse(error)) {
    return (
      <div>
        <h1>{error.status}</h1>
        <h2>{error.statusText}</h2>
      </div>
    )
  }
}
