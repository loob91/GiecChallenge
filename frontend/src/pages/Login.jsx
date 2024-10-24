import React from "react";
import { useState, useEffect } from "react";
import { FaSignInAlt } from 'react-icons/fa'
import { toast } from 'react-toastify'
import {useSelector, useDispatch } from 'react-redux'
import {login, reset } from '../features/auth/authSlice'
import {useNavigate} from 'react-router-dom'
import { useTranslation } from "react-i18next";

function Login() {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
    })

    const {email, password} = formData

    const dispatch = useDispatch()
    const navigate = useNavigate()

    const {user, isLoading, isSuccess, isError, message} = useSelector((state) => state.auth)

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess && user) 
            navigate('/')
        dispatch(reset())
    }, [user, isLoading, isError, isSuccess, message, navigate, dispatch])

    const onSubmit = async(e) => {
        e.preventDefault()

        const userData = {
            email,
            password
        }

        await dispatch(login(userData))

        if (!isLoading && isSuccess)
            navigate('/')
    }

    const onChange = (e) => {
        setFormData((prevState) => ({
            ...prevState,
            [e.target.id] : e.target.value,
        }))
    }

    const { t } = useTranslation(["Login"]);

    if (isLoading)
        return (<div>
            Loading...
        </div>)

    return (<>
        <section className="heading">
            <h1>
                <FaSignInAlt /> {t("login")}
            </h1>
            <p>{t("fillLogin")}</p>
        </section>
        <section className="form">
            <form onSubmit={onSubmit}>
                <div className="form-group">
                    <input type="email" name="email" id="email" value={email} className="form-control width-100" placeholder={t("emailPlaceholder")} onChange={onChange} required />
                </div>
                <div className="form-group">
                    <input type="password" name="password" id="password" value={password} className="form-control width-100" placeholder={t("passwordPlaceholder")} onChange={onChange} required />
                </div>
                <div className="form-group">
                    <button className="btn btn-block">
                        {t("submit")}
                    </button>
                </div>
            </form>
        </section>
    </>);
}

export default Login;
