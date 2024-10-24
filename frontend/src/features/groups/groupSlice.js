import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import groupservice from "./groupService";

const initialState = {
    group: {},
    groups: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: ''
}

export const create = createAsyncThunk('groups/create', async(group, thunkAPI) => {
    try {
        return await groupservice.create(group, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getgroups = createAsyncThunk('groups/getgroups', async(_, thunkAPI) => {
    try {
        return await groupservice.getgroups(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getgroupsbyname = createAsyncThunk('groups/getgroupsbyname', async(name, thunkAPI) => {
    try {
        return await groupservice.getgroupsbyname(name, getLanguage(thunkAPI), getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getgroup = createAsyncThunk('groups/getgroup', async(id, thunkAPI) => {
    try {
        return await groupservice.getgroup(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const updategroup = createAsyncThunk('groups/update', async(group, thunkAPI) => {
    try {
        return await groupservice.updategroup(group, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const deletegroup = createAsyncThunk('groups/delete', async(groupId, thunkAPI) => {
    try {
        return await groupservice.deletegroup(groupId, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const groupslice = createSlice({
    name: 'group',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.group = {}
            state.CO2Emissions = 0
            state.groups = []
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
        .addCase(getgroups.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getgroups.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.groups = action.payload
        })
        .addCase(getgroups.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getgroupsbyname.fulfilled, (state, action) => {
            state.groups = action.payload
        })
        .addCase(getgroup.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getgroup.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.group = action.payload
        })
        .addCase(getgroup.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(updategroup.pending, (state) => {
            state.isLoading = true
        })
        .addCase(updategroup.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
        })
        .addCase(updategroup.rejected, (state, action) => {
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

export const {reset, resetIsSuccess} = groupslice.actions

export default groupslice.reducer
