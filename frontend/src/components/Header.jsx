import React from 'react';
import {FaSignInAlt, FaSignOutAlt, FaUser} from 'react-icons/fa'
import {Link, useNavigate} from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux';
import {logout, reset} from '../features/auth/authSlice'
import LanguageSelector from './LanguageSelector';
import { useTranslation } from 'react-i18next';

function Header() {
    const {user} = useSelector(state => state.auth)
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const { t } = useTranslation(["Header"]);

    const onLogout = async() => {
        await dispatch(logout())
        dispatch(reset())
        navigate('/login')
    }

    return (
        <div className="header">
            <div className="logo">
                <Link to='/'>GiecChallenge</Link>
            </div>
                <ul>
                {
                    !user ?  (
                        <>
                            <li>
                                <Link to='/login'>
                                    <FaSignInAlt /> {t("login")}
                                </Link>
                            </li>
                            <li>
                                <Link to='/register'>
                                    <FaUser /> {t("register")}
                                </Link>
                            </li>
                            <li>
                                <LanguageSelector />
                            </li>
                        </>) : (
                        <>
                            <li>
                                <a onClick={onLogout} href='/'>
                                    <FaSignOutAlt /> {t("logout")}
                                </a>
                            </li>
                            <li>
                                <LanguageSelector />
                            </li>
                        </>
                        )
                }
                </ul>
        </div>
    );
}

export default Header;
