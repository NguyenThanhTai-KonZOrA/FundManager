import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  define: {
    // Inject build timestamp as APP_VERSION
    __APP_VERSION__: JSON.stringify(new Date().getTime().toString()),
  },
  build: {
    // Generate unique filenames for each build to prevent caching
    rollupOptions: {
      output: {
        // Hashed filenames are enough for cache-busting and are fully supported.
        chunkFileNames: 'assets/[name]-[hash].js',
        entryFileNames: 'assets/[name]-[hash].js',
        assetFileNames: 'assets/[name]-[hash].[ext]'
      }
    }
  }
})
