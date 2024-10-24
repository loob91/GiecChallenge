import {useDispatch, useSelector} from 'react-redux'
import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';
import {toast} from 'react-toastify'
import Modal from 'react-modal'
import Spinner from "../components/Spinner";
import { LineProductPreFill } from "../components/SearchBox/LineProductPreFill";
import { getpurchase, reset, updatepurchase, deletelinepurchase} from '../features/purchases/purchaseSlice'
import { useTranslation } from 'react-i18next';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { FaShoppingCart } from 'react-icons/fa';

Modal.setAppElement('#root');

function Purchase() {
    const {purchase, isLoading, isError, message} = useSelector((state) => state.purchase)
    const dispatch = useDispatch()
    const params = useParams()
    const [purchaseUser, setPurchaseUser] = useState()
    const [datePurchase, setDatePurchase] = useState()
    const [lineProductsData, setLineProductsData] = useState([]);
    const { t } = useTranslation(["Purchase"]);

    useEffect(() => {
        if (isError) {
            toast.error(message)
        }
    }, [dispatch, isError, message])

    useEffect(() => {
        dispatch(getpurchase(params.id))
    }, [])

    useEffect(() => {
        setPurchaseUser(purchase)
        if (Object.keys(purchase).length > 0) {
            if (lineProductsData.length == 0) {
                setDatePurchase(new Date(Date.parse(purchase.datePurchase)))
                setLineProductsData(purchase.products.map((p) => {
                    return {
                        id : p.id,
                        currencyIsoCode : p.currencyIsoCode,
                        productId : p.productId,
                        price : p.price,
                        quantity : p.quantity,
                        translation : p.product,
                        selectedValue : p.id
                    }
                }))
            }
        }
    }, [purchase])

    const addOrModifyLineProduct = (lineProduct) => {
        let existingLine = lineProductsData.findIndex(lpd => lpd.id === lineProduct.id);
        if (existingLine === -1)
            setLineProductsData(lineProductsData.concat(lineProduct))
        else {
            lineProductsData[existingLine] = lineProduct
            setLineProductsData(lineProductsData)
        }
    }

    const deleteLineProduct = (p) => {
        let existingLine = lineProductsData.findIndex(lpd => lpd.id === p);
        dispatch(reset());
        if (isNaN(p)) {
            dispatch(deletelinepurchase(p))
        }
        lineProductsData.splice(existingLine, 1)
    }

    const addNewProduct = () => {
        setLineProductsData(lineProductsData.concat([
            {
                id : lineProductsData.length,
                currencyIsoCode : lineProductsData[0].currencyIsoCode,
                productId : '',
                price : 0,
                quantity : 0,
                translation : '',
                selectedValue : ''
            }
        ]));
    }

    const onSubmit = (e) => {
        e.preventDefault()

        const purchaseToUpdate = {
            id: purchase.id,
            products: lineProductsData.flatMap((productLine) => 
            !Array.isArray(productLine.selectedValue) ?
                {
                    id: productLine.id,
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

    if (isLoading)
        return <Spinner />

    if (isError)
        return <h3>An error occured</h3>

    return (
        <>
            {
                Object.keys(purchase).length > 0 ? (
                <>
                    <form onSubmit={onSubmit} className="form-group">
                        <div className='width-100'>
                            <div className='inlineflex'>
                                <div className='width-40'>{t("datePurchase")}</div>
                                <div className='width-60'><DatePicker dateFormat="dd/MM/yyyy" selected={datePurchase} className="form-control width-100" onChange={(date) => setDatePurchase(date)}  /></div>
                            </div>
                        </div>
                        <div className='width-100'>
                            <div className='inlineflex'>
                                <div className='width-80'>{t("CO2Cost")}</div>
                                <div className='width-20'>{parseFloat(purchaseUser.cO2Cost).toFixed(2)}</div>
                            </div>
                        </div>
                        <div className='width-100'>
                            <div className='inlineflex'>
                                <div className='width-80'>{t("WaterCost")}</div>
                                <div className='width-20'>{parseFloat(purchaseUser.waterCost).toFixed(2)}</div>
                            </div>
                        </div>
                        {
                            lineProductsData.length > 0 ?
                            lineProductsData.map((item) => (
                                <LineProductPreFill key={item.id} product={item}  nameSelect={item.id} onChange={(e) => addOrModifyLineProduct(e)} toDelete={(e) => deleteLineProduct(e)}/>
                            )) :
                            <></>
                        }
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
            ) : <></>}
        </>
    );
}

export default Purchase;
