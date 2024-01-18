import {cssBundleHref} from '@remix-run/css-bundle'
import type {LoaderFunctionArgs, MetaDescriptor} from '@remix-run/node'
import {
  Links,
  LiveReload,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  json,
} from '@remix-run/react'
import styles from './globals.css'
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
  const theme = useTheme()
  const nonce = useNonce()

  return (
    <html lang="en" className={theme}>
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
      <body className="bg-background text-foreground">
        <ThemeSwitch />
        <main className="p-6">
          <Outlet />
        </main>
        <ScrollRestoration />
        <Scripts />
        <LiveReload />
      </body>
    </html>
  )
}
