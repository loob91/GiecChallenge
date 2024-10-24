import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import {createLaRuche, reset, resetIsSuccess} from '../../features/purchases/purchaseSlice'
import { toast } from 'react-toastify'
import Spinner from '../../components/Spinner'
import { useTranslation } from 'react-i18next';

export const NewPurchaseLaRuche = ({ datePurchase, purchaseSubmittedForLaRuche }) => {
    const {isLoading, isError, isSuccess, message, purchasesToRename} = useSelector((state) => state.purchase)
    const [command, setCommand] = useState('')
    const { t } = useTranslation(["NewPurchase"]);

    const dispatch = useDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess && !isLoading) {
            if (purchasesToRename.length === 0) {
                dispatch(reset())
                navigate('/')
            }
            else {
                dispatch(resetIsSuccess())
                purchaseSubmittedForLaRuche(purchasesToRename)
            }
        }
    }, [isError, isSuccess, message, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const purchase = {
            datePurchase: datePurchase,
            command: command
        }

        dispatch(createLaRuche(purchase))
    }

    if (isLoading)
        return <Spinner />

    return (
        <>
            <form onSubmit={onSubmit} className="form-group">
                <div className="form-group">
                    <label htmlFor="command">{t("command")}</label>
                    <textarea name="command" id="command" className='form-control width-100' value={command} placeholder={t("command")} onChange={(e) => setCommand(e.target.value)}></textarea>
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

export default NewPurchaseLaRuche