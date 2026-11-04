import { useId, useState, type FormEvent } from 'react'
import { toast } from 'sonner'
import { AvisoError, Campo, conError, PieFormulario } from '@/componentes/Formulario'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import type { Doctor, Especialidad } from '@/lib/tipos'
import { useEspecialidades, useGuardarDoctor, useGuardarEspecialidad } from './consultas'

const CORREO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

// ============================================
// Doctor
// ============================================

export function FormularioDoctor({
  abierto,
  alCerrar,
  doctor,
}: {
  abierto: boolean
  alCerrar: () => void
  doctor: Doctor | null
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="max-h-[calc(100dvh-2rem)] gap-0 overflow-y-auto p-0 sm:max-w-xl">
        {abierto ? <CuerpoDoctor doctor={doctor} alCerrar={alCerrar} /> : null}
      </DialogContent>
    </Dialog>
  )
}

function CuerpoDoctor({ doctor, alCerrar }: { doctor: Doctor | null; alCerrar: () => void }) {
  const especialidades = useEspecialidades()
  const guardar = useGuardarDoctor()
  const [v, setV] = useState({
    nombres: doctor?.nombres ?? '',
    apellidos: doctor?.apellidos ?? '',
    telefono: doctor?.telefono ?? '',
    email: doctor?.email ?? '',
    horarioAtencion: doctor?.horarioAtencion ?? '',
    idEspecialidad: doctor?.idEspecialidad ?? 0,
  })
  const cambiar = <K extends keyof typeof v>(campo: K, valor: (typeof v)[K]) => setV((a) => ({ ...a, [campo]: valor }))
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const base = useId()
  const id = (campo: string) => `${base}-${campo}`

  const errores: Partial<Record<keyof typeof v, string>> = {}
  if (!v.nombres.trim()) errores.nombres = 'Escribe el nombre.'
  if (!v.apellidos.trim()) errores.apellidos = 'Escribe los apellidos.'
  if (!v.idEspecialidad) errores.idEspecialidad = 'Elige la especialidad.'
  if (!v.telefono.trim()) errores.telefono = 'Escribe el teléfono.'
  if (!v.email.trim()) errores.email = 'Escribe el correo.'
  else if (!CORREO.test(v.email.trim())) errores.email = 'El correo no tiene un formato válido.'
  const error = (campo: keyof typeof v) => (mostrarErrores ? errores[campo] : undefined)

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (Object.keys(errores).length > 0) return
    guardar.mutate(
      {
        id: doctor?.id,
        datos: {
          nombres: v.nombres.trim(),
          apellidos: v.apellidos.trim(),
          telefono: v.telefono.trim(),
          email: v.email.trim(),
          horarioAtencion: v.horarioAtencion.trim(),
          idEspecialidad: v.idEspecialidad,
        },
      },
      {
        onSuccess: () => {
          toast.success(doctor ? 'Datos del doctor actualizados.' : 'Doctor registrado.')
          alCerrar()
        },
        onError: setErrorApi,
      },
    )
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {doctor ? 'Editar doctor' : 'Registrar doctor'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          Para que el doctor entre al sistema, después crea su cuenta en Usuarios.
        </DialogDescription>
      </DialogHeader>
      <div className="grid gap-5 p-6 sm:grid-cols-2">
        {errorApi ? (
          <div className="sm:col-span-2">
            <AvisoError error={errorApi} titulo="Revisa los datos del doctor:" />
          </div>
        ) : null}
        <Campo etiqueta="Nombres" htmlFor={id('nombres')} error={error('nombres')}>
          <Input
            id={id('nombres')}
            value={v.nombres}
            maxLength={100}
            onChange={(e) => cambiar('nombres', e.target.value)}
            {...conError(id('nombres'), error('nombres'))}
          />
        </Campo>
        <Campo etiqueta="Apellidos" htmlFor={id('apellidos')} error={error('apellidos')}>
          <Input
            id={id('apellidos')}
            value={v.apellidos}
            maxLength={100}
            onChange={(e) => cambiar('apellidos', e.target.value)}
            {...conError(id('apellidos'), error('apellidos'))}
          />
        </Campo>
        <Campo etiqueta="Especialidad" htmlFor={id('especialidad')} error={error('idEspecialidad')} className="sm:col-span-2">
          <Select
            value={v.idEspecialidad ? String(v.idEspecialidad) : ''}
            onValueChange={(valor) => cambiar('idEspecialidad', Number(valor))}
          >
            <SelectTrigger
              id={id('especialidad')}
              className="h-9 w-full"
              {...conError(id('especialidad'), error('idEspecialidad'))}
            >
              <SelectValue placeholder={especialidades.isPending ? 'Cargando…' : 'Elige una especialidad'} />
            </SelectTrigger>
            <SelectContent>
              {(especialidades.data ?? []).map((e) => (
                <SelectItem key={e.id} value={String(e.id)}>
                  {e.nombre}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Campo>
        <Campo etiqueta="Teléfono" htmlFor={id('telefono')} error={error('telefono')}>
          <Input
            id={id('telefono')}
            type="tel"
            value={v.telefono}
            maxLength={20}
            onChange={(e) => cambiar('telefono', e.target.value)}
            {...conError(id('telefono'), error('telefono'))}
          />
        </Campo>
        <Campo etiqueta="Correo" htmlFor={id('email')} error={error('email')}>
          <Input
            id={id('email')}
            type="email"
            value={v.email}
            maxLength={100}
            onChange={(e) => cambiar('email', e.target.value)}
            {...conError(id('email'), error('email'))}
          />
        </Campo>
        <Campo etiqueta="Horario de atención" opcional htmlFor={id('horario')} className="sm:col-span-2">
          <Input
            id={id('horario')}
            value={v.horarioAtencion}
            maxLength={200}
            placeholder="Lunes a viernes de 9:00 a 17:00"
            onChange={(e) => cambiar('horarioAtencion', e.target.value)}
          />
        </Campo>
      </div>
      <PieFormulario
        enviando={guardar.isPending}
        textoEnviar={doctor ? 'Guardar cambios' : 'Registrar doctor'}
        alCancelar={alCerrar}
      />
    </form>
  )
}

// ============================================
// Especialidad
// ============================================

export function FormularioEspecialidad({
  abierto,
  alCerrar,
  especialidad,
}: {
  abierto: boolean
  alCerrar: () => void
  especialidad: Especialidad | null
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="gap-0 p-0 sm:max-w-md">
        {abierto ? <CuerpoEspecialidad especialidad={especialidad} alCerrar={alCerrar} /> : null}
      </DialogContent>
    </Dialog>
  )
}

function CuerpoEspecialidad({ especialidad, alCerrar }: { especialidad: Especialidad | null; alCerrar: () => void }) {
  const guardar = useGuardarEspecialidad()
  const [nombre, setNombre] = useState(especialidad?.nombre ?? '')
  const [descripcion, setDescripcion] = useState(especialidad?.descripcion ?? '')
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const base = useId()
  const errorNombre = mostrarErrores && !nombre.trim() ? 'Escribe el nombre.' : undefined

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (!nombre.trim()) return
    guardar.mutate(
      { id: especialidad?.id, datos: { nombre: nombre.trim(), descripcion: descripcion.trim() } },
      {
        onSuccess: () => {
          toast.success(especialidad ? 'Especialidad actualizada.' : 'Especialidad creada.')
          alCerrar()
        },
        onError: setErrorApi,
      },
    )
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {especialidad ? 'Editar especialidad' : 'Nueva especialidad'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">Las especialidades agrupan a los doctores en los reportes.</DialogDescription>
      </DialogHeader>
      <div className="grid gap-5 p-6">
        {errorApi ? <AvisoError error={errorApi} /> : null}
        <Campo etiqueta="Nombre" htmlFor={`${base}-nombre`} error={errorNombre}>
          <Input
            id={`${base}-nombre`}
            value={nombre}
            maxLength={100}
            onChange={(e) => setNombre(e.target.value)}
            {...conError(`${base}-nombre`, errorNombre)}
          />
        </Campo>
        <Campo etiqueta="Descripción" opcional htmlFor={`${base}-descripcion`}>
          <Textarea
            id={`${base}-descripcion`}
            value={descripcion}
            rows={3}
            maxLength={500}
            onChange={(e) => setDescripcion(e.target.value)}
          />
        </Campo>
      </div>
      <PieFormulario
        enviando={guardar.isPending}
        textoEnviar={especialidad ? 'Guardar cambios' : 'Crear especialidad'}
        alCancelar={alCerrar}
      />
    </form>
  )
}
