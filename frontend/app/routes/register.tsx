import {Button} from '~/components/ui/button'
import {Input} from '~/components/ui/input'
import {Label} from '~/components/ui/label'

export default function Component() {
  return (
    <div className="mx-auto max-w-sm">
      <div className="mb-6 text-center">
        <h1 className="text-3xl font-bold">Register</h1>
        <p className="text-muted-foreground">
          Enter your information to create an account
        </p>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="email">Email</Label>
          <Input
            id="email"
            name="email"
            type="email"
            placeholder="m@example.com"
            required
          />
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="name">Name</Label>
          <Input id="name" name="name" placeholder="John Doe" />
        </div>
      </div>

      <div className="mt-4 flex flex-col gap-2">
        <Label htmlFor="phone">Phone</Label>
        <Input id="phone" name="phone" placeholder="+1 234 567 890" />
      </div>

      <Button className="mt-4 w-full" type="submit">
        Register
      </Button>
    </div>
  )
}
