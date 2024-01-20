import {z} from 'zod'

const envVariables = z.object({
  NODE_ENV: z.enum(['development', 'production', 'test']),
  SESSION_SECRET: z.string().trim().min(1),
})

// Just in case somewhere around the codebase we access process.env
declare global {
  namespace NodeJS {
    interface ProcessEnv extends z.infer<typeof envVariables> {}
  }
}

try {
  envVariables.parse(process.env)
} catch (error) {
  if (error instanceof z.ZodError) {
    const {fieldErrors} = error.flatten()
    const errorMessage = Object.entries(fieldErrors)
      .map(([field, errors]) =>
        errors ? `${field}: ${errors.join(', ')}` : field,
      )
      .join('\n  ')
    throw new Error(`Missing environment variables:\n  ${errorMessage}`)
  }
}

export const ENV = envVariables.parse(process.env)
