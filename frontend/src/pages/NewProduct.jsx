import React, { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import Spinner from '../components/Spinner'
import BackButton from '../components/BackButton'
import { useTranslation } from 'react-i18next';
import "react-datepicker/dist/react-datepicker.css";
import NewGroup from '../components/NewProduct/NewGroup/NewGroup';
import NewSubGroup from '../components/NewProduct/NewSubGroup/NewSubGroup';
import NewProductItem from '../components/NewProduct/NewProductItem/NewProductItem';
import { getlanguages } from '../features/languages/languageSlice';

function NewProduct() {
    const { languages, isLoading } = useSelector((state) => state.language)
    const [languagesOption, setLanguageOptions] = useState([])
    const { i18n } = useTranslation();

    const dispatch = useDispatch()

    useEffect(() => {
        setLanguageOptions(languages.map((languageOption) => {
            if (languageOption.names.length > 0) {
                return <option value={languageOption.id} key={languageOption.id}>{languageOption.names.findIndex(function(name) { return name.language === i18n.language }) === -1 ? languageOption.names[0].name : languageOption.names.find(function(name) { return name.language === i18n.language }).name}</option>;
            }
        }));
    }, [languages, i18n.language])

    useEffect(() => {
        dispatch(getlanguages())
    }, [dispatch])

    if (isLoading)
        return <Spinner />
    else 
        return (
            <>
                <BackButton url='/' />
                <NewGroup languagesOption={languagesOption} />
                <NewSubGroup languagesOption={languagesOption} />
                <NewProductItem languagesOption={languagesOption} />
            </>
        );
}

export default NewProduct