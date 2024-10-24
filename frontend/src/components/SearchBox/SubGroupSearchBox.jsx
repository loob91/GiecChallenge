import React from 'react';
import Select from 'react-select';
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useTranslation } from 'react-i18next';
import {reset, getsubgroupsbyname} from '../../features/subgroups/subGroupSlice'

export const SubGroupSearchBox = ({ nameSelect, onChange }) => {
    const { subgroups } = useSelector((state) => state.subgroup)
    const [options, setOptions] = useState([]);
    const [inputValueChanged, setInputValueChanged] = useState('');
    const [selectedValue, setSelectedValue] = useState([]);
    const dispatch = useDispatch()
    const { t } = useTranslation(["NewProduct"]);

    useEffect(() => {  
      if (inputValueChanged !== null && inputValueChanged.length > 2) {
        dispatch(getsubgroupsbyname(inputValueChanged));
      }
    }, [dispatch, inputValueChanged]);

    useEffect(() => {  
        onChange(selectedValue)
        dispatch(reset())
    }, [selectedValue]);

    useEffect(() => {  
      setOptions(subgroups.slice(0,10).map((p) => {
          return {
              value: p.id, 
              label: p.names.find(function(name) { return name.language === localStorage.getItem('i18nextLng') }).name + ' - ' + p.group
          };
      }));
    }, [subgroups]);

    const styles = {
        container: base => ({
          ...base,
          flex: 1
        })
      };
    return (
      <>
        <Select
            className="width-100"
            styles={styles}
            options={options}
            name={nameSelect}
            placeholder={t("subGroup")}
            closeMenuOnSelect={true}
            onChange={(e) => {
                setSelectedValue(e.value);
            }}
            onInputChange={(e) => {
                setInputValueChanged(e)
            }}
        />
      </>
    );
  };