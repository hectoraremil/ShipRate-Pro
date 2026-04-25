const isDevelopment = import.meta.env.DEV

function write(level, message, context) {
  if (!isDevelopment) {
    return
  }

  const payload = context ? [message, context] : [message]
  console[level](...payload)
}

export const logger = {
  info(message, context) {
    write('info', message, context)
  },
  warn(message, context) {
    write('warn', message, context)
  },
  error(message, context) {
    write('error', message, context)
  },
}
