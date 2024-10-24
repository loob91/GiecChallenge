import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import purchaseService from "./purchaseService";

const initialState = {
    purchase: {},
    purchases: [],
    CO2Emissions: 0,
    purchasesToRename: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: ''
}

export const create = createAsyncThunk('purchases/create', async(purchase, thunkAPI) => {
    try {
        return await purchaseService.create(purchase, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const createLaRuche = createAsyncThunk('purchases/createLaRuche', async(purchase, thunkAPI) => {
    try {
        return await purchaseService.createLaRuche(purchase, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getpurchases = createAsyncThunk('purchases/getpurchases', async(_, thunkAPI) => {
    try {
        return await purchaseService.getpurchases(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getpurchasesbydate = createAsyncThunk('purchases/getpurchasebydate', async(dates, thunkAPI) => {
    try {
        return await purchaseService.getpurchasesbydate(dates, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getCO2bydate = createAsyncThunk('purchases/getCO2bydate', async(dates, thunkAPI) => {
    try {
        return await purchaseService.getCO2bydate(dates, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getpurchase = createAsyncThunk('purchases/getpurchase', async(id, thunkAPI) => {
    try {
        return await purchaseService.getpurchase(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const updatepurchase = createAsyncThunk('purchases/update', async(purchase, thunkAPI) => {
    try {
        return await purchaseService.updatepurchase(purchase, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const deletepurchase = createAsyncThunk('purchases/delete', async(purchaseId, thunkAPI) => {
    try {
        return await purchaseService.deletepurchase(purchaseId, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const deletelinepurchase = createAsyncThunk('purchases/deletelinepurchase', async(linePurchaseId, thunkAPI) => {
    try {
        return await purchaseService.deletelinepurchase(linePurchaseId, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const purchaseSlice = createSlice({
    name: 'purchase',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.purchase = {}
            state.CO2Emissions = 0
            state.purchases = []
            state.purchasesToRename = []
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
        .addCase(createLaRuche.pending, (state) => {
            state.isLoading = true
        })
        .addCase(createLaRuche.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.purchasesToRename = action.payload.productsToTranslate
            state.purchase = action.payload
        })
        .addCase(createLaRuche.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.message = action.payload
            state.isError = true
        })
        .addCase(getpurchases.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getpurchases.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.purchases = action.payload
        })
        .addCase(getpurchases.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getpurchasesbydate.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getpurchasesbydate.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.purchases = action.payload
        })
        .addCase(getpurchasesbydate.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getCO2bydate.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getCO2bydate.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.CO2Emissions = action.payload
        })
        .addCase(getCO2bydate.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getpurchase.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getpurchase.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.purchase = action.payload
        })
        .addCase(getpurchase.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(updatepurchase.pending, (state) => {
            state.isLoading = true
        })
        .addCase(updatepurchase.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
        })
        .addCase(updatepurchase.rejected, (state, action) => {
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

export const {reset, resetIsSuccess} = purchaseSlice.actions

export default purchaseSlice.reducer
