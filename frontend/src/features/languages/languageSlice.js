import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import languageservice from "./languageService";

const initialState = {
    language: {},
    languages: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: ''
}

export const create = createAsyncThunk('languages/create', async(language, thunkAPI) => {
    try {
        return await languageservice.create(language, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getlanguages = createAsyncThunk('languages/getlanguages', async(_, thunkAPI) => {
    try {
        return await languageservice.getlanguages(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getlanguage = createAsyncThunk('languages/getlanguage', async(id, thunkAPI) => {
    try {
        return await languageservice.getlanguage(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const updatelanguage = createAsyncThunk('languages/update', async(language, thunkAPI) => {
    try {
        return await languageservice.updatelanguage(language, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const deletelanguage = createAsyncThunk('languages/delete', async(languageId, thunkAPI) => {
    try {
        return await languageservice.deletelanguage(languageId, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const languageslice = createSlice({
    name: 'language',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.language = {}
            state.languages = []
            state.groupsToRename = []
        },
        resetIsSuccess: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
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
        .addCase(getlanguages.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getlanguages.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.languages = action.payload
        })
        .addCase(getlanguages.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getlanguage.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getlanguage.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.language = action.payload
        })
        .addCase(getlanguage.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(updatelanguage.pending, (state) => {
            state.isLoading = true
        })
        .addCase(updatelanguage.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
        })
        .addCase(updatelanguage.rejected, (state, action) => {
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

export const {reset, resetIsSuccess} = languageslice.actions

export default languageslice.reducer
