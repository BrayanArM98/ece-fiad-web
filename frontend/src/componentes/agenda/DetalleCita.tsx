import { Check, CircleCheckBig, Pencil, UserX, X } from 'lucide-react'
import { useState, type ReactNode } from 'react'
import { toast } from 'sonner'
import { InsigniaEstado } from '@/componentes/InsigniaEstado'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import { Button } from '@/components/ui/button'
import { Sheet, SheetContent, SheetDescription, SheetFooter, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { mensajeDeError } from '@/lib/api'
import { aFecha, formatearFechaLarga, formatearHora, mayuscula } from '@/lib/formato'
import type { Cita, ResultadoCita } from '@/lib/tipos'
import { useCancelarCita, useRegistrarResultado } from './consultas'

const RESULTADOS: { estado: ResultadoCita; texto: string; icono: typeof Check; aviso: string }[] = [
  { estado: 'Confirmada', texto: 'Confirmar', icono: Check, aviso: 'Cita confirmada.' },
  { estado: 'Completada', texto: 'Completada', icono: CircleCheckBig, aviso: 'Cita marcada como completada.' },
  { estado: 'NoAsistio', texto: 'No asistió', icono: UserX, aviso: 'Se registró que el paciente no asistió.' },
]

/**
 * Panel lateral con el detalle de una cita y sus acciones (specs/frontend-pantallas.md, "Agenda").
 * - Confirmar se puede antes de la cita; Completada y No asistió, a partir de su hora.
 * - Editar y cancelar solo para administración y recepción, y solo si la cita sigue abierta.
 */
export function DetalleCita({
  cita,
  alCerrar,
  puedeAgendar,
  alEditar,
}: {
  cita: Cita | null
  alCerrar: () => void
  puedeAgendar: boolean
  alEditar: (cita: Cita) => void
}) {
  const [confirmarCancelacion, setConfirmarCancelacion] = useState(false)
  const registrar = useRegistrarResultado()
  const cancelar = useCancelarCita()

  const abierta = cita !== null && (cita.estado === 'Pendiente' || cita.estado === 'Confirmada')
  const yaPaso = cita !== null && aFecha(cita.fechaHora) <= new Date()

  const registrarResultado = (estado: ResultadoCita, aviso: string) => {
    if (!cita) return
    registrar.mutate(
      { id: cita.id, estado },
      {
        onSuccess: () => toast.success(aviso),
        onError: (error) => toast.error(mensajeDeError(error)),
      },
    )
  }

  const cancelarCita = () => {
    if (!cita) return
    cancelar.mutate(cita.id, {
      onSuccess: () => {
        toast.success('Cita cancelada.')
        setConfirmarCancelacion(false)
        alCerrar()
      },
      onError: (error) => toast.error(mensajeDeError(error)),
    })
  }

  return (
    <>
      <Sheet open={cita !== null} onOpenChange={(abierto) => !abierto && alCerrar()}>
        <SheetContent className="gap-0 data-[side=right]:w-full data-[side=right]:sm:max-w-md">
          {cita ? (
            <>
              <SheetHeader className="gap-3 border-b border-linea p-6">
                <InsigniaEstado estado={cita.estado} className="w-fit" />
                <div className="grid gap-1">
                  <SheetTitle className="font-heading text-xl font-semibold">{cita.nombrePaciente}</SheetTitle>
                  <SheetDescription className="text-pizarra">
                    {mayuscula(formatearFechaLarga(cita.fechaHora))} · {formatearHora(cita.fechaHora)}
                  </SheetDescription>
                </div>
              </SheetHeader>

              <div className="grid flex-1 content-start gap-8 overflow-y-auto p-6">
                <dl className="grid gap-4">
                  <Dato titulo="Doctor">{cita.nombreDoctor}</Dato>
                  <Dato titulo="Motivo">{cita.motivo || 'Sin motivo registrado'}</Dato>
                  <Dato titulo="Notas">{cita.notas || 'Sin notas'}</Dato>
                </dl>

                {cita.estado !== 'Cancelada' ? (
                  <section aria-labelledby="titulo-resultado" className="grid gap-3">
                    <div>
                      <h3 id="titulo-resultado" className="font-heading font-semibold">
                        Resultado de la cita
                      </h3>
                      <p className="text-pizarra">
                        {yaPaso
                          ? 'Registra si el paciente asistió.'
                          : 'Puedes confirmarla ahora; el resultado se registra a partir de su hora.'}
                      </p>
                    </div>
                    <div className="grid gap-2 sm:grid-cols-3">
                      {RESULTADOS.map(({ estado, texto, icono: Icono, aviso }) => {
                        const actual = cita.estado === estado
                        const permitido = estado === 'Confirmada' ? cita.estado === 'Pendiente' : yaPaso
                        return (
                          <Button
                            key={estado}
                            variant={actual ? 'secondary' : 'outline'}
                            disabled={actual || !permitido || registrar.isPending}
                            aria-pressed={actual}
                            onClick={() => registrarResultado(estado, aviso)}
                          >
                            <Icono aria-hidden="true" />
                            {texto}
                          </Button>
                        )
                      })}
                    </div>
                  </section>
                ) : null}
              </div>

              {puedeAgendar && abierta ? (
                <SheetFooter className="flex-row justify-between border-t border-linea p-6">
                  <Button variant="destructive" onClick={() => setConfirmarCancelacion(true)}>
                    <X aria-hidden="true" />
                    Cancelar cita
                  </Button>
                  <Button variant="outline" onClick={() => alEditar(cita)}>
                    <Pencil aria-hidden="true" />
                    Editar
                  </Button>
                </SheetFooter>
              ) : null}
            </>
          ) : null}
        </SheetContent>
      </Sheet>

      <AlertDialog open={confirmarCancelacion} onOpenChange={setConfirmarCancelacion}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>¿Cancelar la cita?</AlertDialogTitle>
            <AlertDialogDescription>
              {cita
                ? `La cita de ${cita.nombrePaciente} del ${formatearFechaLarga(cita.fechaHora)} a las ${formatearHora(cita.fechaHora)} se cancelará y el horario quedará libre.`
                : null}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={cancelar.isPending}>Conservar cita</AlertDialogCancel>
            <AlertDialogAction
              variant="destructive"
              disabled={cancelar.isPending}
              onClick={(evento) => {
                // Se mantiene abierto hasta que la API responda
                evento.preventDefault()
                cancelarCita()
              }}
            >
              {cancelar.isPending ? 'Cancelando…' : 'Cancelar cita'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}

function Dato({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <div className="grid gap-0.5">
      <dt className="text-xs font-medium text-pizarra">{titulo}</dt>
      <dd className="whitespace-pre-line text-tinta">{children}</dd>
    </div>
  )
}
