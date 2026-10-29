import { cn } from '@/lib/utils'

/** Símbolo de ECE-FIAD: una carpeta de expediente con la cruz médica. */
export function SimboloMarca({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 32 32" aria-hidden="true" className={cn('size-8', className)}>
      <path
        fill="currentColor"
        d="M3 7a3 3 0 0 1 3-3h6.2a3 3 0 0 1 2.1.9L16.4 7H26a3 3 0 0 1 3 3v15a3 3 0 0 1-3 3H6a3 3 0 0 1-3-3z"
      />
      <path fill="#fff" d="M14.5 12h3v4h4v3h-4v4h-3v-4h-4v-3h4z" />
    </svg>
  )
}
