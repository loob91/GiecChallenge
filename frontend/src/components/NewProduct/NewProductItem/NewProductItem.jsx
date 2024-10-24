import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import {reset, create} from '../../../features/products/productSlice'
import {FaShoppingCart} from 'react-icons/fa'
import { LineProductItem } from './LineProductItem'
import { toast } from 'react-toastify'
import Spinner from '../../../components/Spinner'
import { useTranslation } from 'react-i18next';
import {SubGroupSearchBox} from '../../SearchBox/SubGroupSearchBox'

export const NewProductItem = ({ languagesOption }) => {
    const {isLoading, isError, isSuccess, message } = useSelector((state) => state.product)
    const [selectedValue, setSelectedValue] = useState([]);
    const [lineProductsData, setLineProductsData] = useState([{ nameSelected: "product0",productName: "", language: "" }])
    const [CO2Emissions, setCO2Emission] = useState('0')
    const [waterEmissions, setWaterEmission] = useState('0')
    const [amortization, setAmortization] = useState('0')
    const { t } = useTranslation(["NewProduct"]);

    const dispatch = useDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess) {
            dispatch(reset())
            setLineProductsData([{nameSelected: "product0", productName: "", language: "" }])
            toast.success(t("productSuccess"))
        }
    }, [isError, isSuccess, message, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const newProduct = {
            group : selectedValue,
            CO2: CO2Emissions,
            water: waterEmissions,
            amortization: amortization,
            names: lineProductsData.map((productLine) => (
                {
                    name: productLine.productName,
                    language: productLine.language
                }
            ))
        }

        dispatch(create(newProduct))
    }

    const addOrModifyLineProduct = (lineProduct) => {
        if (languagesOption[0] !== undefined && lineProduct.language === '')
            lineProduct.language = languagesOption[0].key;
        let existingLine = lineProductsData.findIndex(lpd => lpd.nameSelected === lineProduct.nameSelected);
        if (existingLine === -1) {
            setLineProductsData(lineProductsData.concat(lineProduct))
        }
        else {
            lineProductsData[existingLine] = lineProduct
            setLineProductsData(lineProductsData)
        }
    }

    const addNewProduct = () => {
        addOrModifyLineProduct(
            { nameSelected: "product" + lineProductsData.length, productName: "", language: "" }
        );
    }

    if (isLoading)
        return <Spinner />

    return (
        <fieldset>
            <legend>{t("newProduct")}</legend>
            <form onSubmit={onSubmit} className="form-group">
                <label htmlFor="product">{t("product")}</label>
                <SubGroupSearchBox key="subGroupSelected" className='mbottom-10 width-100' nameSelect="subGroupSelected" onChange={(e) => setSelectedValue(e)} />
                <label htmlFor="CO2Emissions">{t("CO2Emissions")}</label>
                <input type="text" key="CO2Emissions" className='mbottom-10 width-100' onChange={(e) => setCO2Emission(e.target.value)} />
                <label htmlFor="WaterEmissions">{t("waterEmissions")}</label>
                <input type="text" key="waterEmissions" className='mbottom-10 width-100'onChange={(e) => setWaterEmission(e.target.value)} />
                <label htmlFor="amortization">{t("amortization")}</label>
                <input type="text" key="amortization" className='mbottom-10 width-100'onChange={(e) => setAmortization(e.target.value)} />
                {
                    lineProductsData.map((item) => (
                        <LineProductItem key={item.nameSelected} languageOptions={languagesOption} nameSelected={item.nameSelected} onChange={(e) => addOrModifyLineProduct(e)}/>
                    ))
                }
                <div>
                    <a onClick={() => addNewProduct()} href="#/" className='btn btn-reverse btn-block'>
                        <FaShoppingCart /> {t("addNewTranslation")}
                    </a>
                </div>
                <div className="form-group">
                    <button className="btn btn-block">
                        {t("submit")}
                    </button>
                </div>
            </form>
        </fieldset>
    );
}

export default NewProductItem