import { useQueryClient } from '@tanstack/react-query'
import { createContext, useCallback, useContext, useMemo, useRef, useState, type ReactNode } from 'react'
import { ErrorApi, configurarApi, pedir } from '@/lib/api'
import type { Sesion, Usuario } from '@/lib/tipos'

/**
 * Sesión del usuario (specs/frontend-pantallas.md, "Reglas de navegación").
 * Se guarda en sessionStorage: al cerrar el navegador la sesión termina,
 * lo adecuado para una computadora compartida en recepción.
 */
const CLAVE = 'ece-fiad.sesion'

interface ValorSesion {
  usuario: Usuario | null
  /** Mensaje para la pantalla de inicio de sesión, por ejemplo cuando la sesión expiró. */
  aviso: string | null
  iniciarSesion: (email: string, contrasena: string) => Promise<void>
  cerrarSesion: (aviso?: string) => void
}

const ContextoSesion = createContext<ValorSesion | null>(null)

function leerSesionGuardada(): Sesion | null {
  try {
    const texto = sessionStorage.getItem(CLAVE)
    if (!texto) return null
    const sesion = JSON.parse(texto) as Sesion
    return new Date(sesion.expiraUtc).getTime() > Date.now() ? sesion : null
  } catch {
    return null
  }
}

export function SesionProvider({ children }: { children: ReactNode }) {
  const clienteConsultas = useQueryClient()
  const [sesion, setSesion] = useState<Sesion | null>(leerSesionGuardada)
  const [aviso, setAviso] = useState<string | null>(null)

  // El cliente de la API lee el token en el momento de cada petición
  const token = useRef(sesion?.token ?? null)
  token.current = sesion?.token ?? null

  const cerrarSesion = useCallback(
    (mensaje?: string) => {
      try {
        sessionStorage.removeItem(CLAVE)
      } catch {
        // Sin acceso al almacenamiento: la sesión solo vivía en memoria
      }
      setSesion(null)
      setAviso(mensaje ?? null)
      clienteConsultas.clear()
    },
    [clienteConsultas],
  )

  // Se configura durante el render (no en un efecto) porque las pantallas hijas
  // piden datos en sus propios efectos, que React ejecuta antes que los del proveedor
  configurarApi({
    obtenerToken: () => token.current,
    alExpirarSesion: () => cerrarSesion('Tu sesión expiró. Inicia sesión de nuevo.'),
  })

  const iniciarSesion = useCallback(async (email: string, contrasena: string) => {
    const nueva = await pedir<Sesion>('/api/Auth/login', { metodo: 'POST', cuerpo: { email, contrasena } })

    if (nueva.usuario.rol === 'Paciente') {
      throw new ErrorApi(403, 'Las cuentas de paciente se usan en la app móvil de ECE-FIAD.')
    }

    try {
      sessionStorage.setItem(CLAVE, JSON.stringify(nueva))
    } catch {
      // Sin almacenamiento la sesión dura mientras la pestaña esté abierta
    }
    setAviso(null)
    setSesion(nueva)
  }, [])

  const valor = useMemo<ValorSesion>(
    () => ({ usuario: sesion?.usuario ?? null, aviso, iniciarSesion, cerrarSesion }),
    [sesion, aviso, iniciarSesion, cerrarSesion],
  )

  return <ContextoSesion.Provider value={valor}>{children}</ContextoSesion.Provider>
}

export function useSesion() {
  const valor = useContext(ContextoSesion)
  if (!valor) throw new Error('useSesion debe usarse dentro de <SesionProvider>')
  return valor
}
