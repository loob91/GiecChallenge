import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import subGroupService from "./subGroupService";

const initialState = {
    subgroup: {},
    subgroups: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: ''
}

export const create = createAsyncThunk('subgroups/create', async(group, thunkAPI) => {
    try {
        return await subGroupService.create(group, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getsubgroups = createAsyncThunk('subgroups/getsubgroups', async(_, thunkAPI) => {
    try {
        return await subGroupService.getsubgroups(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getsubgroupsbyname = createAsyncThunk('subgroups/getsubgroupsbyname', async(name, thunkAPI) => {
    try {
        return await subGroupService.getsubgroupsbyname(name, getLanguage(thunkAPI), getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getsubgroup = createAsyncThunk('subgroups/subgetgroup', async(id, thunkAPI) => {
    try {
        return await subGroupService.getsubgroup(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const updatesubgroup = createAsyncThunk('subgroups/update', async(group, thunkAPI) => {
    try {
        return await subGroupService.updatesubgroup(group, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const deletesubgroup = createAsyncThunk('subgroups/delete', async(groupId, thunkAPI) => {
    try {
        return await subGroupService.deletesubgroup(groupId, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const subgroupslice = createSlice({
    name: 'group',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.subgroup = {}
            state.subgroups = []
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
        .addCase(getsubgroups.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getsubgroups.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.subgroups = action.payload
        })
        .addCase(getsubgroups.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getsubgroupsbyname.fulfilled, (state, action) => {
            state.subgroups = action.payload
        })
        .addCase(getsubgroup.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getsubgroup.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.subgroup = action.payload
        })
        .addCase(getsubgroup.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(updatesubgroup.pending, (state) => {
            state.isLoading = true
        })
        .addCase(updatesubgroup.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
        })
        .addCase(updatesubgroup.rejected, (state, action) => {
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

const getLanguage = () => {
    return localStorage.getItem('i18nextLng');
}

export const {reset, resetIsSuccess} = subgroupslice.actions

export default subgroupslice.reducer
