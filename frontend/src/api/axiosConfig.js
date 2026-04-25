import axios from 'axios'
import { logger } from '../utils/logger'

const baseURL = import.meta.env.VITE_API_BASE_URL

if (!baseURL) {
  throw new Error('La variable VITE_API_BASE_URL no esta configurada.')
}

const axiosInstance = axios.create({
  baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

axiosInstance.interceptors.request.use(
  (config) => {
    logger.info('HTTP request started', {
      method: config.method,
      url: `${config.baseURL}${config.url}`,
      params: config.params,
      data: config.data,
    })

    return config
  },
  (error) => {
    logger.error('HTTP request configuration failed', { error })
    return Promise.reject(error)
  },
)

axiosInstance.interceptors.response.use(
  (response) => {
    logger.info('HTTP request completed', {
      method: response.config.method,
      url: `${response.config.baseURL}${response.config.url}`,
      status: response.status,
    })

    return response
  },
  (error) => {
    logger.error('HTTP request failed', {
      method: error?.config?.method,
      url: error?.config ? `${error.config.baseURL}${error.config.url}` : undefined,
      status: error?.response?.status,
      response: error?.response?.data,
    })

    return Promise.reject(error)
  },
)

export default axiosInstance
