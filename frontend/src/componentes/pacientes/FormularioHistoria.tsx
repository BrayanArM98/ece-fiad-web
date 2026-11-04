import { useId, useState, type FormEvent } from 'react'
import { toast } from 'sonner'
import { AvisoError, Campo, PieFormulario } from '@/componentes/Formulario'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Textarea } from '@/components/ui/textarea'
import type { HistoriaClinica } from '@/lib/tipos'
import { useGuardarHistoria } from './consultas'

/** Abrir o editar la historia clínica de un paciente: alergias y antecedentes (máximo 500 caracteres cada uno). */
export function FormularioHistoria({
  abierto,
  alCerrar,
  idPaciente,
  historia,
}: {
  abierto: boolean
  alCerrar: () => void
  idPaciente: number
  historia: HistoriaClinica | null
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="max-h-[calc(100dvh-2rem)] gap-0 overflow-y-auto p-0 sm:max-w-lg">
        {abierto ? <Cuerpo idPaciente={idPaciente} historia={historia} alCerrar={alCerrar} /> : null}
      </DialogContent>
    </Dialog>
  )
}

function Cuerpo({
  idPaciente,
  historia,
  alCerrar,
}: {
  idPaciente: number
  historia: HistoriaClinica | null
  alCerrar: () => void
}) {
  const [alergias, setAlergias] = useState(historia?.alergias ?? '')
  const [personales, setPersonales] = useState(historia?.antecedentesPersonales ?? '')
  const [familiares, setFamiliares] = useState(historia?.antecedentesFamiliares ?? '')
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const guardar = useGuardarHistoria(idPaciente)
  const base = useId()

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setErrorApi(null)
    guardar.mutate(
      {
        historia,
        datos: {
          alergias: alergias.trim(),
          antecedentesPersonales: personales.trim(),
          antecedentesFamiliares: familiares.trim(),
        },
      },
      {
        onSuccess: () => {
          toast.success(historia ? 'Historia clínica actualizada.' : 'Historia clínica abierta.')
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
          {historia ? 'Editar historia clínica' : 'Abrir historia clínica'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          Las alergias aparecen en el encabezado del expediente para que siempre estén a la vista.
        </DialogDescription>
      </DialogHeader>
      <div className="grid gap-5 p-6">
        {errorApi ? <AvisoError error={errorApi} /> : null}
        <Campo
          etiqueta="Alergias"
          htmlFor={`${base}-alergias`}
          ayuda="Déjalo vacío si no tiene alergias conocidas."
        >
          <Textarea
            id={`${base}-alergias`}
            value={alergias}
            rows={2}
            maxLength={500}
            placeholder="Por ejemplo: penicilina, mariscos"
            onChange={(e) => setAlergias(e.target.value)}
          />
        </Campo>
        <Campo etiqueta="Antecedentes personales" opcional htmlFor={`${base}-personales`}>
          <Textarea
            id={`${base}-personales`}
            value={personales}
            rows={3}
            maxLength={500}
            placeholder="Enfermedades, cirugías, hábitos y medicamentos de uso continuo"
            onChange={(e) => setPersonales(e.target.value)}
          />
        </Campo>
        <Campo etiqueta="Antecedentes familiares" opcional htmlFor={`${base}-familiares`}>
          <Textarea
            id={`${base}-familiares`}
            value={familiares}
            rows={3}
            maxLength={500}
            placeholder="Diabetes, hipertensión, cáncer u otras enfermedades en la familia"
            onChange={(e) => setFamiliares(e.target.value)}
          />
        </Campo>
      </div>
      <PieFormulario
        enviando={guardar.isPending}
        textoEnviar={historia ? 'Guardar cambios' : 'Abrir historia'}
        alCancelar={alCerrar}
      />
    </form>
  )
}
