import React from 'react';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom';
import {reset, create} from '../../../features/subgroups/subGroupSlice'
import {FaShoppingCart} from 'react-icons/fa'
import { LineSubGroup } from './LineSubGroup'
import { toast } from 'react-toastify'
import Spinner from '../../../components/Spinner'
import { useTranslation } from 'react-i18next';
import {GroupSearchBox} from '../../SearchBox/GroupSearchBox'

export const NewSubGroup = ({ languagesOption }) => {
    const {isLoading, isError, isSuccess, message } = useSelector((state) => state.group)
    const [subgroups, setSubGroups] = useState([{key: 0, name: "subgroup0" }])
    const [selectedValue, setSelectedValue] = useState([]);
    const { t } = useTranslation(["NewProduct"]);
    const [lineSubGroupsData, setLineSubGroupsData] = useState([])

    const dispatch = useDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (isError)
            toast.error(message)
        if (isSuccess) {
            dispatch(reset())
            setSubGroups([{key: 0, name: "subgroup0" }])
            setLineSubGroupsData([])
            toast.success(t("subGroupSuccess"))
        }
    }, [isError, isSuccess, message, navigate, dispatch])

    const onSubmit = (e) => {
        e.preventDefault()

        const newSubGroup = {
            group : selectedValue,
            names: lineSubGroupsData.map((subGroupLine) => (
                {
                    name: subGroupLine.subGroup,
                    language: subGroupLine.language
                }
            ))
        }

        dispatch(create(newSubGroup))
    }

    const addOrModifyLineLanguage = (lineLanguage) => {
        if (languagesOption[0] !== undefined && lineLanguage.language === '')
            lineLanguage.language = languagesOption[0].key;
        let existingLine = lineSubGroupsData.findIndex(lpd => lpd.nameSelect === lineLanguage.nameSelect);
        if (existingLine === -1)
            setLineSubGroupsData(lineSubGroupsData.concat(lineLanguage))
        else {
            lineSubGroupsData[existingLine] = lineLanguage
            setLineSubGroupsData(lineSubGroupsData)
        }
    }

    const addNewGroup = () => {
        setSubGroups(subgroups.concat([
            {key: subgroups.length, name: "subgroup" + subgroups.length }
        ]));
    }

    if (isLoading)
        return <Spinner />

    return (
        <fieldset>
            <legend>{t("newSubGroup")}</legend>
            <form onSubmit={onSubmit} className="form-group">
                <label htmlFor="group">{t("group")}</label>
                <GroupSearchBox key="groupSelected" className='mbottom-10' nameSelect="groupSelected" onChange={(e) => setSelectedValue(e)} />
                {
                    subgroups.map((item) => (
                        <LineSubGroup key={item.key} languageOptions={languagesOption} nameSelect={item.key} onChange={(e) => addOrModifyLineLanguage(e)}/>
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

export default NewSubGroup