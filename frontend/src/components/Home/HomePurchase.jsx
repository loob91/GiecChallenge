import React, { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import Spinner from '../../components/Spinner'
import ProgressBar from 'react-bootstrap/ProgressBar';
import { reset, getpurchasesbydate, deletepurchase, getCO2bydate} from '../../features/purchases/purchaseSlice'
import {LinePurchase} from './LinePurchase'
import { useTranslation } from 'react-i18next';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

export const  HomePurchase = () => {
    const { isLoading, purchases, CO2Emissions } = useSelector((state) => state.purchase)
    const [startDate, setStartDate] = useState(new Date());
    const [endDate, setEndDate] = useState(new Date());
    const [purchasesUser, setPurchasesUser] = useState([]);
    const [CO2EmissionsPurcentage, setCO2EmissionsPurcentage] = useState(0.0);
    const { t } = useTranslation(["Home"]);

    const dispatch = useDispatch()

    useEffect(() => {
        setPurchasesUser(purchases);
        setCO2EmissionsPurcentage(CO2Emissions / (2500 / 365 * (endDate.getDate() - startDate.getDate() + 1)) * 100);
    }, [purchases, CO2Emissions])

    useEffect(() => {
        changeDate();
    }, [startDate, endDate])

    function changeDate() {
        dispatch(reset())
        dispatch(getpurchasesbydate({ 
            startDate: startDate.toLocaleDateString("es-CL"), 
            endDate: endDate.toLocaleDateString("es-CL")
        }));
        dispatch(getCO2bydate({ 
            startDate: startDate.toLocaleDateString("es-CL"), 
            endDate: endDate.toLocaleDateString("es-CL")
        }));
    }

    const deletePurchase = (p) => {
        dispatch(reset());
        dispatch(deletepurchase(p)).then(
            changeDate()
        );
    }

    if (isLoading)
        return <Spinner />
    else
        return (
            <>
                <div className='mbottom-10'>
                    <ProgressBar now={CO2EmissionsPurcentage} label={`${CO2EmissionsPurcentage}%`} />
                </div>
                <div style={{display: "flex"}} className="form-group">
                    <DatePicker dateFormat="dd/MM/yyyy" selected={startDate} className="form-control" onChange={(date) => {setStartDate(date);}}  />
                    <DatePicker dateFormat="dd/MM/yyyy" selected={endDate} className="form-control" onChange={(date) => {setEndDate(date);}}  />
                </div>
                <div className='flex'>
                    <div className='form-group width-20'>{t("date")}</div>
                    <div className='form-group width-20'>{t("CO2Cost")}</div>
                    <div className='form-group width-20'>{t("WaterCost")}</div>
                </div>
                {
                    purchasesUser.map((purchaseUser, index) => {
                        return (
                            <LinePurchase key={index} purchaseUser={purchaseUser} onChange={(p) => deletePurchase(p)} />
                        )                    
                    })
                }
            </>
        );
}

export default HomePurchase