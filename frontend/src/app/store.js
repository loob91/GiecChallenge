import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import productReducer from '../features/products/productSlice';
import purchaseReducer from '../features/purchases/purchaseSlice';
import currencyReducer from '../features/currencies/currencySlice';
import languageReducer from '../features/languages/languageSlice';
import groupReducer from '../features/groups/groupSlice';
import subGroupReducer from '../features/subgroups/subGroupSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    product: productReducer,
    purchase: purchaseReducer,
    currency: currencyReducer,
    language: languageReducer,
    group: groupReducer,
    subgroup: subGroupReducer
  },
});
