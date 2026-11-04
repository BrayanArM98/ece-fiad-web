import { useId, useState, type FormEvent, type ReactNode } from 'react'
import { toast } from 'sonner'
import { AvisoError, Campo, conError, PieFormulario } from '@/componentes/Formulario'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { TEXTO_DOCUMENTO, TEXTO_GENERO, TEXTO_GRUPO } from '@/lib/catalogos'
import { aFechaApi } from '@/lib/formato'
import type { Genero, GrupoSanguineo, Paciente, TipoDocumento } from '@/lib/tipos'
import { useEditarPaciente, useRegistrarPaciente } from './consultas'

const CORREO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

/**
 * Registrar o editar un paciente (specs/frontend-pantallas.md, "Pacientes").
 * Al editar, la API solo permite cambiar nombre, contacto y dirección; el resto se muestra fijo.
 */
export function FormularioPaciente({
  abierto,
  alCerrar,
  paciente,
  alGuardar,
}: {
  abierto: boolean
  alCerrar: () => void
  paciente?: Paciente | null
  alGuardar?: (paciente: Paciente) => void
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="max-h-[calc(100dvh-2rem)] gap-0 overflow-y-auto p-0 sm:max-w-xl">
        {abierto ? <Cuerpo paciente={paciente ?? null} alCerrar={alCerrar} alGuardar={alGuardar} /> : null}
      </DialogContent>
    </Dialog>
  )
}

function Cuerpo({
  paciente,
  alCerrar,
  alGuardar,
}: {
  paciente: Paciente | null
  alCerrar: () => void
  alGuardar?: (paciente: Paciente) => void
}) {
  const editando = paciente !== null
  const [v, setV] = useState({
    nombres: paciente?.nombres ?? '',
    apellidos: paciente?.apellidos ?? '',
    tipoDocumento: (paciente?.tipoDocumento ?? 'Cedula') as TipoDocumento,
    numeroDocumento: paciente?.numeroDocumento ?? '',
    fechaNacimiento: paciente ? paciente.fechaNacimiento.slice(0, 10) : '',
    genero: (paciente?.genero ?? '') as Genero | '',
    grupoSanguineo: (paciente?.grupoSanguineo ?? '') as GrupoSanguineo | '',
    telefono: paciente?.telefono ?? '',
    email: paciente?.email ?? '',
    direccion: paciente?.direccion ?? '',
  })
  const cambiar = <K extends keyof typeof v>(campo: K, valor: (typeof v)[K]) => setV((a) => ({ ...a, [campo]: valor }))
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const registrar = useRegistrarPaciente()
  const editar = useEditarPaciente()
  const enviando = registrar.isPending || editar.isPending

  const base = useId()
  const id = (campo: string) => `${base}-${campo}`

  const hoy = aFechaApi(new Date())
  const minimo = aFechaApi(new Date(new Date().getFullYear() - 120, new Date().getMonth(), new Date().getDate() + 1))

  const errores: Partial<Record<keyof typeof v, string>> = {}
  if (!v.nombres.trim()) errores.nombres = 'Escribe los nombres.'
  if (!v.apellidos.trim()) errores.apellidos = 'Escribe los apellidos.'
  if (!editando) {
    if (!v.numeroDocumento.trim()) errores.numeroDocumento = 'Escribe el número de documento.'
    if (!v.fechaNacimiento) errores.fechaNacimiento = 'Indica la fecha de nacimiento.'
    else if (v.fechaNacimiento >= hoy) errores.fechaNacimiento = 'Debe ser anterior a hoy.'
    else if (v.fechaNacimiento < minimo) errores.fechaNacimiento = 'La edad máxima es de 120 años.'
    if (!v.genero) errores.genero = 'Elige el género.'
    if (!v.grupoSanguineo) errores.grupoSanguineo = 'Elige el grupo sanguíneo.'
  }
  if (!v.email.trim()) errores.email = 'Escribe el correo.'
  else if (!CORREO.test(v.email.trim())) errores.email = 'El correo no tiene un formato válido.'
  const error = (campo: keyof typeof v) => (mostrarErrores ? errores[campo] : undefined)

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (Object.keys(errores).length > 0) return

    const contacto = {
      nombres: v.nombres.trim(),
      apellidos: v.apellidos.trim(),
      telefono: v.telefono.trim(),
      email: v.email.trim(),
      direccion: v.direccion.trim(),
    }
    const opciones = {
      onSuccess: (guardado: Paciente) => {
        toast.success(editando ? 'Datos del paciente actualizados.' : `${guardado.nombres} ${guardado.apellidos} quedó registrado.`)
        alGuardar?.(guardado)
        alCerrar()
      },
      onError: setErrorApi,
    }

    if (paciente) editar.mutate({ id: paciente.id, datos: contacto }, opciones)
    else
      registrar.mutate(
        {
          ...contacto,
          tipoDocumento: v.tipoDocumento,
          numeroDocumento: v.numeroDocumento.trim(),
          fechaNacimiento: `${v.fechaNacimiento}T00:00:00`,
          genero: v.genero as Genero,
          grupoSanguineo: v.grupoSanguineo as GrupoSanguineo,
        },
        opciones,
      )
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {editando ? 'Editar datos del paciente' : 'Registrar paciente'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          {editando
            ? 'El documento, la fecha de nacimiento, el género y el grupo sanguíneo no se modifican.'
            : 'Datos de identificación y de contacto del paciente.'}
        </DialogDescription>
      </DialogHeader>

      <div className="grid gap-5 p-6 sm:grid-cols-2">
        {errorApi ? (
          <div className="sm:col-span-2">
            <AvisoError error={errorApi} titulo="Revisa los datos del paciente:" />
          </div>
        ) : null}

        <Campo etiqueta="Nombres" htmlFor={id('nombres')} error={error('nombres')}>
          <Input
            id={id('nombres')}
            value={v.nombres}
            maxLength={100}
            autoComplete="off"
            onChange={(e) => cambiar('nombres', e.target.value)}
            {...conError(id('nombres'), error('nombres'))}
          />
        </Campo>
        <Campo etiqueta="Apellidos" htmlFor={id('apellidos')} error={error('apellidos')}>
          <Input
            id={id('apellidos')}
            value={v.apellidos}
            maxLength={100}
            autoComplete="off"
            onChange={(e) => cambiar('apellidos', e.target.value)}
            {...conError(id('apellidos'), error('apellidos'))}
          />
        </Campo>

        {editando ? (
          <dl className="grid grid-cols-2 gap-4 rounded-md border border-linea bg-papel p-4 sm:col-span-2 sm:grid-cols-4">
            <Fijo titulo="Documento">
              {TEXTO_DOCUMENTO[paciente.tipoDocumento]} {paciente.numeroDocumento}
            </Fijo>
            <Fijo titulo="Nacimiento">{paciente.fechaNacimiento.slice(0, 10).split('-').reverse().join('/')}</Fijo>
            <Fijo titulo="Género">{TEXTO_GENERO[paciente.genero]}</Fijo>
            <Fijo titulo="Grupo sanguíneo">{TEXTO_GRUPO[paciente.grupoSanguineo]}</Fijo>
          </dl>
        ) : (
          <>
            <Campo etiqueta="Tipo de documento" htmlFor={id('tipo')}>
              <Select value={v.tipoDocumento} onValueChange={(t) => cambiar('tipoDocumento', t as TipoDocumento)}>
                <SelectTrigger id={id('tipo')} className="h-9 w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {(Object.keys(TEXTO_DOCUMENTO) as TipoDocumento[]).map((t) => (
                    <SelectItem key={t} value={t}>
                      {TEXTO_DOCUMENTO[t]}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Campo>
            <Campo etiqueta="Número de documento" htmlFor={id('documento')} error={error('numeroDocumento')}>
              <Input
                id={id('documento')}
                value={v.numeroDocumento}
                maxLength={20}
                autoComplete="off"
                onChange={(e) => cambiar('numeroDocumento', e.target.value)}
                {...conError(id('documento'), error('numeroDocumento'))}
              />
            </Campo>
            <Campo etiqueta="Fecha de nacimiento" htmlFor={id('nacimiento')} error={error('fechaNacimiento')}>
              <Input
                id={id('nacimiento')}
                type="date"
                value={v.fechaNacimiento}
                min={minimo}
                max={hoy}
                onChange={(e) => cambiar('fechaNacimiento', e.target.value)}
                {...conError(id('nacimiento'), error('fechaNacimiento'))}
              />
            </Campo>
            <div className="grid grid-cols-2 gap-4">
              <Campo etiqueta="Género" htmlFor={id('genero')} error={error('genero')}>
                <Select value={v.genero} onValueChange={(g) => cambiar('genero', g as Genero)}>
                  <SelectTrigger id={id('genero')} className="h-9 w-full" {...conError(id('genero'), error('genero'))}>
                    <SelectValue placeholder="Elige" />
                  </SelectTrigger>
                  <SelectContent>
                    {(Object.keys(TEXTO_GENERO) as Genero[]).map((g) => (
                      <SelectItem key={g} value={g}>
                        {TEXTO_GENERO[g]}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </Campo>
              <Campo etiqueta="Grupo sanguíneo" htmlFor={id('grupo')} error={error('grupoSanguineo')}>
                <Select value={v.grupoSanguineo} onValueChange={(g) => cambiar('grupoSanguineo', g as GrupoSanguineo)}>
                  <SelectTrigger
                    id={id('grupo')}
                    className="h-9 w-full"
                    {...conError(id('grupo'), error('grupoSanguineo'))}
                  >
                    <SelectValue placeholder="Elige" />
                  </SelectTrigger>
                  <SelectContent>
                    {(Object.keys(TEXTO_GRUPO) as GrupoSanguineo[]).map((g) => (
                      <SelectItem key={g} value={g}>
                        {TEXTO_GRUPO[g]}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </Campo>
            </div>
          </>
        )}

        <Campo etiqueta="Teléfono" opcional htmlFor={id('telefono')}>
          <Input
            id={id('telefono')}
            type="tel"
            value={v.telefono}
            maxLength={15}
            onChange={(e) => cambiar('telefono', e.target.value)}
          />
        </Campo>
        <Campo etiqueta="Correo" htmlFor={id('email')} error={error('email')}>
          <Input
            id={id('email')}
            type="email"
            value={v.email}
            maxLength={150}
            onChange={(e) => cambiar('email', e.target.value)}
            {...conError(id('email'), error('email'))}
          />
        </Campo>
        <Campo etiqueta="Dirección" opcional htmlFor={id('direccion')} className="sm:col-span-2">
          <Textarea
            id={id('direccion')}
            value={v.direccion}
            rows={2}
            maxLength={250}
            onChange={(e) => cambiar('direccion', e.target.value)}
          />
        </Campo>
      </div>

      <PieFormulario
        enviando={enviando}
        textoEnviar={editando ? 'Guardar cambios' : 'Registrar paciente'}
        alCancelar={alCerrar}
      />
    </form>
  )
}

function Fijo({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <div className="grid gap-0.5">
      <dt className="text-xs text-pizarra">{titulo}</dt>
      <dd className="cifras text-tinta">{children}</dd>
    </div>
  )
}
