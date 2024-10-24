import React, { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { reset} from '../features/purchases/purchaseSlice'
import {getcurrencies} from '../features/currencies/currencySlice'
import { toast } from 'react-toastify'
import Spinner from '../components/Spinner'
import BackButton from '../components/BackButton'
import { useTranslation } from 'react-i18next';
import { NewPurchaseManual } from '../components/NewPurchaseTypes/NewPurchaseManual';
import { NewPurchaseLaRuche } from '../components/NewPurchaseTypes/NewPurchaseLaRuche';
import { NewPurchaseLaRucheUnknownProduct } from '../components/NewPurchaseTypes/NewPurchaseLaRucheUnknownProduct';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

function NewPurchase() {
    const { currencies, isLoading, isError, message } = useSelector((state) => state.currency)
    const [typeOfPurchase, setTypeOfPurchase] = useState('manual')
    const [datePurchase, setDatePurchase] = useState(new Date())
    const [currency, setCurrency] = useState('EUR');
    const [currencyOptions, setCurrencyOptions] = useState([]);
    const [laRucheUnknownProduct, setLaRucheUnknownProduct] = useState([]);
    const { t } = useTranslation(["NewPurchase"]);
    const { i18n } = useTranslation();

    const dispatch = useDispatch()

    useEffect(() => {
        setCurrencyOptions(currencies.map((currencyOption) => {
            if (currencyOption.isoCode === 'EUR')
                setCurrency(currencyOption.id)
            return <option value={currencyOption.id} key={currencyOption.id}>{currencyOption.names.find(function(name) { return name.language === i18n.language }).name}</option>;
        }));
    }, [currencies])

    useEffect(() => {
        dispatch(reset())  
        if (isError)
            toast.error(message)
    }, [isError])

    useEffect(() => {
        dispatch(getcurrencies())
    }, [])

    useEffect(() => {

    }, [isLoading])


    if (isLoading)
        return <Spinner />
    else 
        return (
            <>
                <BackButton url='/' />
                <section className="heading">
                    <h1>{t("title")}</h1>
                </section>
                <section className="form">
                    <div className="form-group">
                        <label htmlFor="name">{t("typeOfPurchase")}</label>
                        <select name="typeOfPurchase" id="typeOfPurchase" value={typeOfPurchase} className="form-control width-100" onChange={(e) => setTypeOfPurchase(e.target.value)}>
                            <option value="manual">{t("manual")}</option>
                            <option value="LaRuche">La Ruche Qui Dit Oui</option>
                        </select>
                        <label htmlFor="name">{t("datePurchase")}</label>
                        <DatePicker dateFormat="dd/MM/yyyy" selected={datePurchase} className="form-control width-100" onChange={(date) => setDatePurchase(date)}  />
                        { typeOfPurchase === "manual" ? 
                            (
                                <>
                                    <label htmlFor="currency">{t("currency")}</label>
                                    <select name="currency" id="currency" value={currency} className="form-control width-100" onChange={(e) => setCurrency(e.target.value)}>
                                        {currencyOptions}
                                    </select>
                                </>
                            ) : 
                            <></>
                        }
                    </div>
                        { typeOfPurchase === "manual" ? 
                            (
                                <NewPurchaseManual datePurchase={datePurchase} currency={currency} />
                            ) :
                            laRucheUnknownProduct.length === 0 ?
                            (
                                <NewPurchaseLaRuche datePurchase={datePurchase} purchaseSubmittedForLaRuche={(e) => setLaRucheUnknownProduct(e)} />
                            ) :
                            (
                                <NewPurchaseLaRucheUnknownProduct listUnknowProduct={laRucheUnknownProduct} />
                            )
                        }
                </section>
            </>
        );
}

export default NewPurchase