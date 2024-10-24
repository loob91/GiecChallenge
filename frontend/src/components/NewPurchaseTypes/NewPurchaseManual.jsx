import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import {create, reset} from '../../features/purchases/purchaseSlice'
import { toast } from 'react-toastify'
import Spinner from '../../components/Spinner'
import { useTranslation } from 'react-i18next';
import { LineProduct } from '../../components/SearchBox/LineProduct';
import {FaShoppingCart} from 'react-icons/fa'
import "react-datepicker/dist/react-datepicker.css";

export const NewPurchaseManual = ({ datePurchase, currency }) => {
    const {isLoading, isError, isSuccess, message } = useSelector((state) => state.purchase)
    const [products, setProducts] = useState([{key: "0", name: "product0"}]);
    const [lineProductsData, setLineProductsData] = useState([]);
    const [nbProduct, setNbProduct] = useState(1);
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
    }, [isError, isSuccess, message, products, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const purchase = {
            datePurchase: datePurchase,
            products: lineProductsData.map((productLine) => (
                {
                    product: productLine.selectedValue,
                    quantity: productLine.quantity,
                    price: productLine.price,
                    currencyIsoCode: currency
                }
            ))
        }

        dispatch(create(purchase))
    }

    const addOrModifyLineProduct = (lineProduct) => {
        let existingLine = lineProductsData.findIndex(lpd => lpd.nameSelect === lineProduct.nameSelect);
        if (existingLine === -1)
            setLineProductsData(lineProductsData.concat(lineProduct))
        else {
            lineProductsData[existingLine] = lineProduct
            setLineProductsData(lineProductsData)
        }
    }

    const addNewProduct = () => {
        setProducts(products.concat([
            {key: nbProduct, name: "product" + nbProduct }
        ]));
        setNbProduct(nbProduct + 1)
    }

    if (isLoading)
        return <Spinner />

    return (
        <>
            <form onSubmit={onSubmit} className="form-group">
                <div className="form-group" id="formProducts">
                    <label>{t("product")}</label>
                    {
                        products.map((item) => (
                            <LineProduct key={item.key} onChange={(e) => addOrModifyLineProduct(e)} nameSelect={item.name}/>
                        ))
                    }
                </div>
                <div>
                    <a onClick={() => addNewProduct()} href="#/" className='btn btn-reverse btn-block'>
                        <FaShoppingCart /> {t("addNewProduct")}
                    </a>
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

export default NewPurchaseManual