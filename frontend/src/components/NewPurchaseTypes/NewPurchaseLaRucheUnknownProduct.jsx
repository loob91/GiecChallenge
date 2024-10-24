import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import { reset, updatepurchase } from '../../features/purchases/purchaseSlice'
import { toast } from 'react-toastify'
import Spinner from '../Spinner'
import { useTranslation } from 'react-i18next';
import { LineProductUnknownPreFill } from '../SearchBox/LineProductUnknownPreFill';
import "react-datepicker/dist/react-datepicker.css";

export const NewPurchaseLaRucheUnknownProduct = ({ listUnknowProduct }) => {
    const {isLoading, isError, isSuccess, message, purchase } = useSelector((state) => state.purchase)
    const [lineProductsData, setLineProductsData] = useState([]);
    const { t } = useTranslation(["NewPurchase"]);

    const dispatch = useDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess) {
            dispatch(reset())
            navigate('/')
        }
    }, [isError, isSuccess, message, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const purchaseToUpdate = {
            id: purchase.id,
            products: lineProductsData.flatMap((productLine) => 
            !Array.isArray(productLine.selectedValue) ?
                {
                    product: productLine.selectedValue,
                    quantity: productLine.quantity,
                    price: productLine.price,
                    currencyIsoCode: productLine.currencyIsoCode,
                    translation: productLine.translation
                }
            : [])
        }

        dispatch(updatepurchase(purchaseToUpdate))
    }

    const addOrModifyLineProduct = (lineProduct) => {
        let existingLine = lineProductsData.findIndex(lpd => lpd.id === lineProduct.id);
        if (existingLine === -1)
            setLineProductsData(lineProductsData.concat(lineProduct))
        else {
            lineProductsData[existingLine] = lineProduct
            setLineProductsData(lineProductsData)
        }
    }

    if (isLoading)
        return <Spinner />

    return (
        <>
            <form onSubmit={onSubmit} className="form-group">
                <div className="form-group" id="formProducts">
                    <label>{t("product")}</label>
                    {
                        listUnknowProduct.map((item) => (
                            <LineProductUnknownPreFill key={item.id} unknowProduct={item}  nameSelect={item.id} onChange={(e) => addOrModifyLineProduct(e)}/>
                        ))
                    }
                </div>
                <div className="form-group">
                    <button className="btn btn-block">
                        {t("submit")}
                    </button>
                </div>
            </form>
        </>
    );
}

export default NewPurchaseLaRucheUnknownProduct