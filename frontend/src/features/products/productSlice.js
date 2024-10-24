import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import productService from "./productService";

const initialState = {
    product: {},
    products: [],
    isError: false,
    isSuccess: false,
    isLoading: false,
    message: '',
}

export const create = createAsyncThunk('products/create', async(product, thunkAPI) => {
    try {
        return await productService.create(product, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getproducts = createAsyncThunk('products/getproducts', async(_, thunkAPI) => {
    try {
        return await productService.getproducts(getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getproduct = createAsyncThunk('products/getproduct', async(id, thunkAPI) => {
    try {
        return await productService.getproduct(id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const getproductbyname = createAsyncThunk('products/getproductbyname', async(name, thunkAPI) => {
    try {
        return await productService.getproductbyname(name, getLanguage(thunkAPI), getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const closeproduct = createAsyncThunk('products/closeproduct', async(_, thunkAPI) => {
    try {
        return await productService.closeproduct(thunkAPI.getState().product.product._id, getToken(thunkAPI))
    } catch (error) {
        const message = (error.response && error.response.data && error.response.data.message) || error.message || error.toString()
        return thunkAPI.rejectWithValue(message)
    }
})

export const productSlice = createSlice({
    name: 'product',
    initialState,
    reducers: {
        reset: (state) => {
            state.isLoading = false
            state.isError = false
            state.isSuccess = false
            state.message = ''
            state.product = {}
            state.products = []
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
        .addCase(getproductbyname.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getproductbyname.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.products = action.payload
        })
        .addCase(getproductbyname.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getproducts.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getproducts.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.products = action.payload
        })
        .addCase(getproducts.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(getproduct.pending, (state) => {
            state.isLoading = true
        })
        .addCase(getproduct.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.product = action.payload
        })
        .addCase(getproduct.rejected, (state, action) => {
            state.isSuccess = false
            state.isLoading = false
            state.isError = true
            state.message = action.payload
        })
        .addCase(closeproduct.pending, (state) => {
            state.isLoading = true
        })
        .addCase(closeproduct.fulfilled, (state, action) => {
            state.isSuccess = true
            state.isLoading = false
            state.isError = false
            state.products.map((product) => product._id === action.payload._id ? action.payload.status = 'closed' : product)
        })
        .addCase(closeproduct.rejected, (state, action) => {
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

export const {reset} = productSlice.actions

export default productSlice.reducer
