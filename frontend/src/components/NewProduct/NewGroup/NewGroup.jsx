import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import {reset, create} from '../../../features/groups/groupSlice'
import {FaShoppingCart} from 'react-icons/fa'
import { LineGroup } from './LineGroup'
import { toast } from 'react-toastify'
import Spinner from '../../../components/Spinner'
import { useTranslation } from 'react-i18next';

export const NewGroup = ({ languagesOption }) => {
    const {isLoading, isError, isSuccess, message } = useSelector((state) => state.group)
    const [groups, setGroups] = useState([{key: 0, name: "group0" }])
    const { t } = useTranslation(["NewProduct"]);
    const [lineGroupsData, setLineGroupsData] = useState([])

    const dispatch = useDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess) {
            dispatch(reset())
            setGroups([{key: 0, name: "group0" }])
            setLineGroupsData([])
            toast.success(t("groupSuccess"))
        }
    }, [isError, isSuccess, message, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const newGroup = {
            names: lineGroupsData.map((groupLine) => (
                {
                    name: groupLine.group,
                    language: groupLine.language
                }
            ))
        }

        dispatch(create(newGroup))
    }

    const addOrModifyLineLanguage = (lineLanguage) => {
        if (languagesOption[0] !== undefined && lineLanguage.language === '')
            lineLanguage.language = languagesOption[0].key;
        let existingLine = lineGroupsData.findIndex(lpd => lpd.nameSelect === lineLanguage.nameSelect);
        if (existingLine === -1)
            setLineGroupsData(lineGroupsData.concat(lineLanguage))
        else {
            lineGroupsData[existingLine] = lineLanguage
            setLineGroupsData(lineGroupsData)
        }
    }

    const addNewGroup = () => {
        setGroups(groups.concat([
            {key: groups.length, name: "group" + groups.length }
        ]));
    }

    if (isLoading)
        return <Spinner />

    return (
        <fieldset>
            <legend>{t("newGroup")}</legend>
            <form onSubmit={onSubmit} className="form-group">
                {
                    groups.map((item) => (
                        <LineGroup key={item.key} languageOptions={languagesOption} nameSelect={item.key} onChange={(e) => addOrModifyLineLanguage(e)}/>
                    ))
                }
                <div>
                    <a onClick={() => addNewGroup()} href="#/" className='btn btn-reverse btn-block'>
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

export default NewGroup