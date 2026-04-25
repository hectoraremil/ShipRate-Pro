import axiosInstance from './axiosConfig'
import { createApiError } from './apiError'

async function request(config) {
  try {
    const response = await axiosInstance(config)
    return response.data
  } catch (error) {
    throw createApiError(error)
  }
}

export const apiClient = {
  get(url, config = {}) {
    return request({ ...config, method: 'get', url })
  },
  post(url, data, config = {}) {
    return request({ ...config, method: 'post', url, data })
  },
}
