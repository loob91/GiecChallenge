import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import currencieservice from "./currencyService";

const initialState = {
    currency: {},
    currencies: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: '',
}

export const create = createAsyncThunk('currencies/create', async(currency, thunkAPI) => {
    try {
        return await currencieservice.create(currency, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getcurrencies = createAsyncThunk('currencies/getcurrencies', async(_, thunkAPI) => {
    try {
        return await currencieservice.getcurrencies(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getcurrency = createAsyncThunk('currencies/getcurrency', async(id, thunkAPI) => {
    try {
        return await currencieservice.getcurrency(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const closecurrency = createAsyncThunk('currencies/closecurrency', async(_, thunkAPI) => {
    try {
        return await currencieservice.closecurrency(thunkAPI.getState().currency.currency._id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const currencySlice = createSlice({
    name: 'currency',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.currency = {}
            state.currencies = []
        }
    },
    extraReducers: (builder) => {
        builder
        .addCase(create.pending, (state) => {
            state.isLoading = true
        })
        .addCase(create.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
        })
        .addCase(create.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.message = action.payload
            state.isError = true
        })
        .addCase(getcurrencies.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getcurrencies.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.currencies = action.payload
        })
        .addCase(getcurrencies.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getcurrency.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getcurrency.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.currency = action.payload
        })
        .addCase(getcurrency.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(closecurrency.pending, (state) => {
            state.isLoading = true
        })
        .addCase(closecurrency.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.currencies.map((currency) => currency._id === action.payload._id ? action.payload.status = 'closed' : currency)
        })
        .addCase(closecurrency.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
    }
})

const getToken = (thunkAPI) => {
    return thunkAPI.getState().auth.user.token
}

export const {reset} = currencySlice.actions

export default currencySlice.reducer
