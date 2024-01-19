import {Outlet} from '@remix-run/react'

export default function AuthLayout() {
  return (
    <div className="mx-auto mt-6 max-w-sm">
      <Outlet />
    </div>
  )
}
