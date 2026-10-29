/**
 * Cliente de la API ECE-FIAD (specs/frontend-pantallas.md, "Reglas de navegación").
 * - Agrega el token de la sesión a cada petición.
 * - Devuelve directamente `datos` cuando la respuesta es exitosa.
 * - Lanza ErrorApi con el mensaje y los errores de la API en cualquier otro caso.
 * - Si la API responde 401 con una sesión abierta, avisa para cerrarla.
 */

const URL_API = (import.meta.env.VITE_API_URL ?? '').replace(/\/+$/, '')

export class ErrorApi extends Error {
  readonly status: number
  readonly errores: string[]

  constructor(status: number, mensaje: string, errores: string[] = []) {
    super(mensaje)
    this.name = 'ErrorApi'
    this.status = status
    this.errores = errores
  }
}

interface Envoltura<T> {
  exitoso: boolean
  mensaje?: string
  datos?: T
  errores?: string[]
}

type Parametros = Record<string, string | number | boolean | null | undefined>

let obtenerToken: () => string | null = () => null
let alExpirarSesion: () => void = () => {}

/** La sesión registra aquí cómo leer el token y qué hacer cuando expira. */
export function configurarApi(opciones: { obtenerToken: () => string | null; alExpirarSesion: () => void }) {
  obtenerToken = opciones.obtenerToken
  alExpirarSesion = opciones.alExpirarSesion
}

function construirUrl(ruta: string, parametros?: Parametros) {
  const url = new URL(`${URL_API}${ruta}`, window.location.origin)
  for (const [clave, valor] of Object.entries(parametros ?? {})) {
    if (valor !== undefined && valor !== null && valor !== '') url.searchParams.set(clave, String(valor))
  }
  return url.toString()
}

async function enviar(ruta: string, metodo: string, cuerpo?: unknown, parametros?: Parametros) {
  const token = obtenerToken()
  const encabezados: Record<string, string> = { Accept: 'application/json' }
  if (cuerpo !== undefined) encabezados['Content-Type'] = 'application/json'
  if (token) encabezados.Authorization = `Bearer ${token}`

  try {
    return await fetch(construirUrl(ruta, parametros), {
      method: metodo,
      headers: encabezados,
      body: cuerpo !== undefined ? JSON.stringify(cuerpo) : undefined,
    })
  } catch {
    throw new ErrorApi(0, 'No hay conexión con el servidor. Revisa tu conexión e intenta de nuevo.')
  }
}

async function leerError(respuesta: Response): Promise<ErrorApi> {
  const cuerpo = (await respuesta.json().catch(() => null)) as Envoltura<unknown> | null

  if (respuesta.status === 401 && obtenerToken()) alExpirarSesion()

  return new ErrorApi(
    respuesta.status,
    cuerpo?.mensaje ?? `El servidor respondió con un error (${respuesta.status}). Intenta de nuevo.`,
    cuerpo?.errores ?? [],
  )
}

export async function pedir<T>(
  ruta: string,
  opciones: { metodo?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'; cuerpo?: unknown; parametros?: Parametros } = {},
): Promise<T> {
  const respuesta = await enviar(ruta, opciones.metodo ?? 'GET', opciones.cuerpo, opciones.parametros)
  if (!respuesta.ok) throw await leerError(respuesta)

  const cuerpo = (await respuesta.json().catch(() => null)) as Envoltura<T> | null
  if (!cuerpo?.exitoso) {
    throw new ErrorApi(respuesta.status, cuerpo?.mensaje ?? 'Respuesta inesperada del servidor.', cuerpo?.errores ?? [])
  }
  return cuerpo.datos as T
}

/** Descarga un archivo (PDF o Excel) con el nombre que indica el servidor. */
export async function descargar(ruta: string, parametros?: Parametros) {
  const respuesta = await enviar(ruta, 'GET', undefined, parametros)
  if (!respuesta.ok) throw await leerError(respuesta)

  const disposicion = respuesta.headers.get('Content-Disposition') ?? ''
  const nombre =
    /filename\*=UTF-8''([^;]+)/i.exec(disposicion)?.[1] ?? /filename="?([^";]+)"?/i.exec(disposicion)?.[1] ?? 'reporte'

  const enlace = document.createElement('a')
  enlace.href = URL.createObjectURL(await respuesta.blob())
  enlace.download = decodeURIComponent(nombre)
  enlace.click()
  URL.revokeObjectURL(enlace.href)
}

/** Mensaje legible de cualquier error, para mostrarlo en pantalla. */
export function mensajeDeError(error: unknown): string {
  if (error instanceof ErrorApi) return error.message
  return 'Ocurrió un error inesperado. Intenta de nuevo.'
}
