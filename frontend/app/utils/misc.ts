import {useRouteLoaderData} from '@remix-run/react'
import type {loader} from '~/root'

export function useRequestInfo() {
  const data = useRouteLoaderData<typeof loader>('root')
  if (!data) throw new Error('Missing request info')
  return data.requestInfo
}

export function useHints() {
  const requestInfo = useRequestInfo()
  return requestInfo.hints
}
