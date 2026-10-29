# ECE-FIAD Web

Frontend del expediente clínico electrónico: React, TypeScript, Vite, Tailwind CSS y shadcn/ui. Consume la API de `../backend`.

## Desarrollo

```bash
npm install
cp .env.example .env.local   # en Windows: copy .env.example .env.local
npm run dev
```

La aplicación abre en `http://localhost:5173` y usa la API indicada en `VITE_API_URL`.

## Especificaciones

El diseño y las pantallas están en [`../specs`](../specs): `frontend-diseno.md` y `frontend-pantallas.md`.
