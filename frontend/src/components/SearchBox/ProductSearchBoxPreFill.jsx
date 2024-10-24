import React from 'react';
import Select from 'react-select';
import {getproductbyname} from '../../features/products/productSlice'
import { useEffect, useState } from 'react';
import {useSelector, useDispatch } from 'react-redux'
import { useTranslation } from 'react-i18next';
import { reset } from '../../features/products/productSlice';

export const ProductSearchBoxPreFill = ({ preSelectedValue, preSelectedInputValue, nameSelect, onChange }) => {
    const { products } = useSelector((state) => state.product)
    const [options, setOptions] = useState([]);
    const [inputValueChanged, setInputValueChanged] = useState(preSelectedInputValue);
    const [selectedValue, setSelectedValue] = useState(preSelectedValue);
    const dispatch = useDispatch()
    const { t } = useTranslation(["LineProduct"]);

    useEffect(() => {  
      if (inputValueChanged !== null && inputValueChanged.length > 2) {
          dispatch(getproductbyname(inputValueChanged));
      }
    }, [dispatch, inputValueChanged]);

    useEffect(() => {  
      dispatch(getproductbyname(inputValueChanged));
    }, []);

    useEffect(() => {  
        onChange(selectedValue)
        dispatch(reset())
    }, [selectedValue]);

    useEffect(() => {  
      setOptions(products.slice(0,10).map((p) => {
          return {
              value: p.id, 
              label: p.names.find(function(name) { return name.language === localStorage.getItem('i18nextLng') }).name + ' - ' + p.group
          };
      }));
    }, [products]);

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
            placeholder={t("product")}
            closeMenuOnSelect={true}
            defaultInputValue={preSelectedInputValue}
            defaultValue={preSelectedValue}
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