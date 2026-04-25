export class ApiError extends Error {
  constructor({
    message,
    status = null,
    validationErrors = {},
    traceId = null,
    originalError = null,
  }) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.validationErrors = validationErrors
    this.traceId = traceId
    this.originalError = originalError
  }
}

function normalizeValidationErrors(validationErrors) {
  if (!validationErrors || typeof validationErrors !== 'object') {
    return {}
  }

  return Object.entries(validationErrors).reduce((accumulator, [field, messages]) => {
    accumulator[field] = Array.isArray(messages) ? messages : [String(messages)]
    return accumulator
  }, {})
}

export function createApiError(error) {
  const responseData = error?.response?.data
  const validationErrors = normalizeValidationErrors(responseData?.errors)
  const firstValidationMessage = Object.values(validationErrors).flat()[0]

  const message =
    firstValidationMessage ||
    responseData?.title ||
    error?.message ||
    'No fue posible completar la solicitud en este momento.'

  return new ApiError({
    message,
    status: error?.response?.status ?? null,
    validationErrors,
    traceId: responseData?.traceId ?? null,
    originalError: error,
  })
}

export function getUserFriendlyErrorMessage(error, fallbackMessage) {
  if (error instanceof ApiError && error.message) {
    return error.message
  }

  return fallbackMessage || 'Ha ocurrido un error inesperado. Intente nuevamente.'
}
