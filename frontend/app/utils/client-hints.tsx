import {getHintUtils} from '@epic-web/client-hints'
import {
  clientHint as colorSchemeHint,
  subscribeToSchemeChange,
} from '@epic-web/client-hints/color-scheme'
import {useRevalidator} from '@remix-run/react'
import * as React from 'react'

const hintsUtils = getHintUtils({theme: colorSchemeHint})
export const {getHints} = hintsUtils

/**
 * @returns inline script element that checks for client hints and sets cookies
 * if they are not set then reloads the page if any cookie was set to an
 * inaccurate value.
 */
export function ClientHintCheck({nonce}: {nonce: string}) {
  const {revalidate} = useRevalidator()

  React.useEffect(
    () => subscribeToSchemeChange(() => revalidate()),
    [revalidate],
  )

  return (
    <script
      nonce={nonce}
      dangerouslySetInnerHTML={{
        __html: hintsUtils.getClientHintCheckScript(),
      }}
    />
  )
}
