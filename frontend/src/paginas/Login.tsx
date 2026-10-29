import { useState, type FormEvent } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router'
import { SimboloMarca } from '@/componentes/Marca'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { mensajeDeError } from '@/lib/api'
import { useSesion } from '@/sesion/SesionContext'

const FORMATO_CORREO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

/** Inicio de sesión (specs/frontend-pantallas.md, "Inicio de sesión"). */
export default function PaginaLogin() {
  const { usuario, aviso, iniciarSesion } = useSesion()
  const navegar = useNavigate()
  const ubicacion = useLocation()
  const destino = (ubicacion.state as { desde?: string } | null)?.desde ?? '/'

  const [email, setEmail] = useState('')
  const [contrasena, setContrasena] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [error, setError] = useState<string | null>(null)

  if (usuario) return <Navigate to={destino} replace />

  const puedeEnviar = FORMATO_CORREO.test(email.trim()) && contrasena.length > 0 && !enviando

  async function alEnviar(evento: FormEvent) {
    evento.preventDefault()
    if (!puedeEnviar) return

    setEnviando(true)
    setError(null)
    try {
      await iniciarSesion(email.trim(), contrasena)
      navegar(destino, { replace: true })
    } catch (e) {
      setError(mensajeDeError(e))
      setEnviando(false)
    }
  }

  return (
    <div className="grid min-h-svh lg:grid-cols-[minmax(0,5fr)_minmax(0,6fr)]">
      <aside className="hidden flex-col justify-between bg-marea-profunda p-12 text-white lg:flex">
        <div className="flex items-center gap-3">
          <SimboloMarca className="size-9 text-white/90" />
          <span className="font-heading text-xl font-semibold">ECE-FIAD</span>
        </div>
        <div className="max-w-md">
          <p className="font-heading text-[2.25rem] leading-[1.15] font-semibold">
            El expediente de cada paciente, la agenda de la clínica y sus reportes, en un solo lugar.
          </p>
          <p className="mt-6 text-white/75">Expediente clínico electrónico para recepción, médicos y administración.</p>
        </div>
        <p className="text-sm text-white/60">Facultad de Ingeniería, Arquitectura y Diseño, UABC</p>
      </aside>

      <main className="flex items-center justify-center px-6 py-12">
        <div className="w-full max-w-sm">
          <div className="mb-8 flex items-center gap-3 lg:hidden">
            <SimboloMarca className="text-marea" />
            <span className="font-heading text-xl font-semibold">ECE-FIAD</span>
          </div>

          <h1 className="text-[1.75rem] font-semibold">Inicia sesión</h1>
          <p className="mt-1 text-pizarra">Usa la cuenta que te asignó la administración de la clínica.</p>

          {aviso ? (
            <p role="status" className="mt-6 rounded-md border border-linea bg-marea-suave px-4 py-3 text-marea-profunda">
              {aviso}
            </p>
          ) : null}

          <form onSubmit={alEnviar} className="mt-8 grid gap-5" noValidate>
            <div className="grid gap-2">
              <Label htmlFor="email">Correo</Label>
              <Input
                id="email"
                type="email"
                autoComplete="username"
                placeholder="nombre@ecefiad.mx"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="h-10"
                autoFocus
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contrasena">Contraseña</Label>
              <Input
                id="contrasena"
                type="password"
                autoComplete="current-password"
                value={contrasena}
                onChange={(e) => setContrasena(e.target.value)}
                className="h-10"
              />
            </div>

            {error ? (
              <p role="alert" className="rounded-md bg-coral-suave px-4 py-3 text-coral">
                {error}
              </p>
            ) : null}

            <Button type="submit" size="lg" className="h-10" disabled={!puedeEnviar}>
              {enviando ? 'Entrando…' : 'Entrar'}
            </Button>
          </form>
        </div>
      </main>
    </div>
  )
}
