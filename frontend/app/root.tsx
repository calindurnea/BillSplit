import {cssBundleHref} from '@remix-run/css-bundle'
import type {LoaderFunctionArgs, MetaDescriptor} from '@remix-run/node'
import {
  Link,
  Links,
  LiveReload,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  json,
} from '@remix-run/react'
import {Button} from './components/ui/button'
import styles from './globals.css'
import {cn} from './lib/utils'
import {ThemeSwitch, useTheme} from './routes/action.set-theme'
import {ClientHintCheck, getHints} from './utils/client-hints'
import {useNonce} from './utils/nonce-provider'
import {getTheme} from './utils/theme.server'

export function links() {
  return [
    {rel: 'stylesheet', href: styles},
    ...(cssBundleHref ? [{rel: 'stylesheet', href: cssBundleHref}] : []),
  ]
}

export function meta(): Array<MetaDescriptor> {
  return [
    {title: 'BillSplit'},
    {name: 'description', content: 'Welcome to BillSplit'},
  ]
}

export async function loader({request}: LoaderFunctionArgs) {
  return json({
    hints: getHints(request),
    theme: getTheme(request),
  })
}

export default function App() {
  return (
    <Document>
      <main className="flex flex-1 flex-col p-6">
        <Outlet />
      </main>
      <ScrollRestoration />
      <Scripts />
      <LiveReload />
    </Document>
  )
}

export function ErrorBoundary() {
  return (
    <Document>
      <main className="flex flex-1 flex-col items-center justify-center">
        <div>
          <h1 className="text-5xl font-bold">Oops!</h1>
          <p className="my-4 text-xl text-muted-foreground">
            An error occurred. Please try again later.
          </p>
          <Link to="/">
            <Button variant="secondary">Back to home</Button>
          </Link>
        </div>
      </main>
    </Document>
  )
}

function Document({children}: {children: React.ReactNode}) {
  const theme = useTheme()
  const nonce = useNonce()

  return (
    <html lang="en" className={cn(theme, 'h-full')}>
      <head>
        <ClientHintCheck nonce={nonce} />
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <meta
          name="color-scheme"
          content={theme === 'dark' ? 'dark light' : 'light dark'}
        />
        <Meta />
        <Links />
      </head>
      <body className="flex h-full flex-col bg-background text-foreground">
        <ThemeSwitch />
        {children}
      </body>
    </html>
  )
}
