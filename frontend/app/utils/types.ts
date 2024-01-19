import {z} from 'zod'

export const entitySchema = z.object({
  id: z.coerce.number(),
})

export const knownErrorSchema = z.object({
  type: z.string(),
  title: z.string(),
  detail: z.string(),
  status: z.number(),
})

export type KnownError = z.infer<typeof knownErrorSchema>
