import React from "react";
import {useNavigate} from 'react-router-dom'
import { useState, useEffect } from "react";
import { FaUser } from 'react-icons/fa'
import { toast } from 'react-toastify'
import {useSelector, useDispatch } from 'react-redux'
import {register, reset} from '../features/auth/authSlice'
import { useTranslation } from "react-i18next";

function Register() {
    const { i18n } = useTranslation();
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        password2: '',
    })

    const {email, password, password2} = formData

    const dispatch = useDispatch()
    const navigate = useNavigate()

    const {user, isLoading, isError, isSuccess, message} = useSelector(state => state.auth)

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess && user) {
            dispatch(reset())
            navigate('/')
        }
        
    }, [user, isLoading, isError, isSuccess, message, navigate, dispatch])

    const onSubmit = async(e) => {
        e.preventDefault()

        if (password !== password2){
            toast.error('Passwords must match')
        }
        else {
            const userData = {
                email,
                password,
                language: i18n.language
            }
            dispatch(register(userData));
            if (!isLoading && isSuccess)
                navigate('/')
        }
    }

    const onChange = (e) => {
        setFormData((prevState) => ({
            ...prevState,
            [e.target.id] : e.target.value,
        }))
    }

    return (<>
        <section className="heading">
            <h1>
                <FaUser /> Register {user}
            </h1>
            <p>Please create an account</p>
        </section>
        <section className="form">
            <form onSubmit={onSubmit}>
                <div className="form-group">
                    <input type="email" name="email" id="email" value={email} className="form-control width-100" placeholder="Enter your email" onChange={onChange} required />
                </div>
                <div className="form-group">
                    <input type="password" name="password" id="password" value={password} className="form-control width-100" placeholder="Enter your password" onChange={onChange} required />
                </div>
                <div className="form-group">
                    <input type="password" name="password2" id="password2" value={password2} className="form-control width-100" placeholder="Re-enter your password" onChange={onChange} required />
                </div>
                <div className="form-group">
                    <button className="btn btn-block">
                        Submit
                    </button>
                </div>
            </form>
        </section>
    </>);
}

export default Register;
